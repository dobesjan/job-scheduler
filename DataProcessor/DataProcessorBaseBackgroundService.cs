using DataProcessor.Configuration;
using JobScheduler;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebMonitoring.Models;

namespace DataProcessor
{
	// communication from web api to job scheduler
	public class DataProcessorBaseBackgroundService<T> : IHostedService, IDisposable where T : IMonitoredEntity
	{
		private readonly ILogger _logger;
		private readonly IServiceProvider _serviceProvider;
		private Timer _timer;
		private CancellationTokenSource _cancellationTokenSource;
		private readonly ConcurrentQueue<T> _queue;
		private Task _dataProcessorTask;
		private readonly AppOptions _appOptions;
		private readonly HttpClient _httpClient;
		private readonly string _pluginName;
		private readonly string _apiEndpoint;
		private readonly JobScheduler.JobScheduler.JobSchedulerClient _jobSchedulerClient;

		public DataProcessorBaseBackgroundService(ILogger logger, IServiceProvider serviceProvider, IOptions<AppOptions> options, string pluginName, string apiEndpoint, JobScheduler.JobScheduler.JobSchedulerClient jobSchedulerClient)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_queue = new ConcurrentQueue<T>();
			_appOptions = options.Value;
			_httpClient = new HttpClient();
			_pluginName = pluginName;
			_apiEndpoint = apiEndpoint;
			_jobSchedulerClient = jobSchedulerClient;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Job Scheduler Background Service is starting.");

			_cancellationTokenSource = new CancellationTokenSource();
			_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_appOptions.Interval));

			return Task.CompletedTask;
		}

		private async void DoWork(object state)
		{
			while (!_cancellationTokenSource.Token.IsCancellationRequested)
			{
				try
				{
					var response = await _httpClient.GetAsync(_apiEndpoint);
					response.EnsureSuccessStatusCode();
					var responseString = await response.Content.ReadAsStringAsync();
					var entities = JsonSerializer.Deserialize<IEnumerable<MonitoredWebPage>>(responseString);

					foreach (var entity in entities)
					{
						_logger.LogInformation($"Retrieved entity '{nameof(T)}' with Id '{entity.Id}'");

						var jobRequest = new GetJobRequest
						{
							EntityId = entity.EntityId
						};

						var job = await _jobSchedulerClient.GetJobAsync(jobRequest);

						if (job == null)
						{
							var scheduleJobRequest = new ScheduleJobRequest
							{
								JobName = entity.Name,
								PluginName = _pluginName,
								Parameters = entity.Parameters,
								Interval = entity.Interval,
								EntityId = entity.EntityId
							};

							_jobSchedulerClient.ScheduleJobAsync(scheduleJobRequest, cancellationToken: _cancellationTokenSource.Token);
						}
					}
				}
				catch (Exception ex)
				{
					_logger.LogError($"Error fetching '{nameof(T)}' entities: {ex.Message}");
				}
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(DataProcessorBaseBackgroundService<T>)} is stopping.");

			_timer?.Change(Timeout.Infinite, 0);
			_cancellationTokenSource.Cancel();

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_httpClient.Dispose();
			_timer?.Dispose();
			_cancellationTokenSource?.Dispose();
		}
	}
}
