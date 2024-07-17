using JobScheduler.PluginSystem;
using JobScheduler.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Grpc.Net.Client;
using JobScheduler;
using JobScheduler.Configuration;
using Microsoft.Extensions.Options;

public class JobSchedulerBackgroundService : IHostedService, IDisposable
{
	private readonly ILogger<JobSchedulerBackgroundService> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly DataProcessor.DataProcessorClient _dataProcessorClient;
	private Timer _timer;
	private CancellationTokenSource _cancellationTokenSource;
	private readonly ConcurrentQueue<(int JobId, string Result)> _jobResultsQueue;
	private Task _resultProcessorTask;
	private readonly AppOptions _appOptions;

	public JobSchedulerBackgroundService(ILogger<JobSchedulerBackgroundService> logger, IServiceProvider serviceProvider, DataProcessor.DataProcessorClient dataProcessorClient, IOptions<AppOptions> options)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_dataProcessorClient = dataProcessorClient;
		_jobResultsQueue = new ConcurrentQueue<(int JobId, string Result)>();
		_appOptions = options.Value;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Job Scheduler Background Service is starting.");

		_cancellationTokenSource = new CancellationTokenSource();
		_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_appOptions.Interval));
		_resultProcessorTask = Task.Run(ProcessJobResultsQueueAsync, _cancellationTokenSource.Token);

		return Task.CompletedTask;
	}

	private async void DoWork(object state)
	{
		using (var scope = _serviceProvider.CreateScope())
		{
			var jobStore = scope.ServiceProvider.GetRequiredService<IJobStore>();
			var pluginManager = scope.ServiceProvider.GetRequiredService<IPluginManager>();

			var tasks = new List<Task>();
			while (!_cancellationTokenSource.Token.IsCancellationRequested)
			{
				var job = await jobStore.GetNextJobAsync();
				if (job == null) break;

				tasks.Add(ProcessJobAsync(job, jobStore, pluginManager));
			}

			await Task.WhenAll(tasks);
		}
	}

	private async Task ProcessJobAsync(Job job, IJobStore jobStore, IPluginManager pluginManager)
	{
		_logger.LogInformation($"Processing job {job.JobId}");

		var plugin = pluginManager.GetPlugin(job.PluginName);
		if (plugin != null)
		{
			try
			{
				var result = await plugin.ExecuteAsync(job.Parameters);
				// Enqueue the result
				_jobResultsQueue.Enqueue((job.JobId, result));
				await jobStore.UpdateJobStatusAsync(job.JobId, "Completed", result);
				_logger.LogInformation($"Job {job.JobId} completed successfully.");
			}
			catch (Exception ex)
			{
				await jobStore.UpdateJobStatusAsync(job.JobId, "Failed", ex.Message);
				_logger.LogError(ex, $"Job {job.JobId} failed.");
			}
		}
	}

	private async Task ProcessJobResultsQueueAsync()
	{
		while (!_cancellationTokenSource.Token.IsCancellationRequested)
		{
			if (_jobResultsQueue.TryDequeue(out var jobResult))
			{
				try
				{
					var request = new SendResultRequest
					{
						JobId = jobResult.JobId,
						Result = jobResult.Result
					};

					var response = await _dataProcessorClient.SendResultAsync(request);
					_logger.LogInformation($"Sent result for job {response.JobId}: {response.Result}");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Failed to send result for job {jobResult.JobId}");
					// Re-enqueue the job result for retrying
					_jobResultsQueue.Enqueue(jobResult);
				}
			}
			else
			{
				// Wait before retrying
				await Task.Delay(1000);
			}
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Job Scheduler Background Service is stopping.");

		_timer?.Change(Timeout.Infinite, 0);
		_cancellationTokenSource.Cancel();

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		_timer?.Dispose();
		_cancellationTokenSource?.Dispose();
	}
}
