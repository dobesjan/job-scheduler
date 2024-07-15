using JobScheduler.PluginSystem;
using JobScheduler.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class JobSchedulerBackgroundService : IHostedService, IDisposable
{
	private readonly ILogger<JobSchedulerBackgroundService> _logger;
	private readonly IServiceProvider _serviceProvider;
	private Timer _timer;
	private CancellationTokenSource _cancellationTokenSource;

	public JobSchedulerBackgroundService(ILogger<JobSchedulerBackgroundService> logger, IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Job Scheduler Background Service is starting.");

		_cancellationTokenSource = new CancellationTokenSource();
		_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

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
				//TODO: Handle statuses
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
