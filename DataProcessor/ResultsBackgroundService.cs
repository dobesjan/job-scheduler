using DataProcessor.Configuration;
using DataProcessor.Models;
using Microsoft.Extensions.DependencyInjection;
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
	// communication from scheduler to api
	public class ResultsBackgroundService : IHostedService, IDisposable
	{
		private readonly ILogger<ResultsBackgroundService> _logger;
		private readonly IServiceProvider _serviceProvider;
		private Timer _timer;
		private CancellationTokenSource _cancellationTokenSource;
		private readonly ConcurrentQueue<ProcessedResult> _jobResultsQueue;
		private Task _resultProcessorTask;
		private readonly AppOptions _appOptions;
		private readonly HttpClient _httpClient;

		public ResultsBackgroundService(ILogger<ResultsBackgroundService> logger, IServiceProvider serviceProvider,  IOptions<AppOptions> options, ConcurrentQueue<ProcessedResult> jobResultsQueue)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_jobResultsQueue = jobResultsQueue;
			_appOptions = options.Value;
			_httpClient = new HttpClient();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			// Receive results from DataProcessorService and enqueu it
			// Consider data decoration in future
			// Retrieve from queue and start sending to web api
			// Send to webapi which will store this data to db

			_logger.LogInformation($"{nameof(ResultsBackgroundService)} is starting.");

			_cancellationTokenSource = new CancellationTokenSource();
			_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_appOptions.Interval));
			_resultProcessorTask = Task.Run(ProcessJobResultsQueueAsync, _cancellationTokenSource.Token);

			return Task.CompletedTask;
		}

		private async void DoWork(object state)
		{
			await ProcessJobResultsQueueAsync();
		}

		private async Task ProcessJobResultsQueueAsync()
		{
			while (!_cancellationTokenSource.Token.IsCancellationRequested)
			{
				if (_jobResultsQueue.TryDequeue(out var jobResult))
				{
					var content = new StringContent(JsonSerializer.Serialize(jobResult), Encoding.UTF8, "application/json");

					var result = jobResult.GetResult();
					HttpResponseMessage response = null;

					switch (result.EntityTypeId)
					{
						case (int)EMonitoredEntityType.WebPage:
							response = await _httpClient.PostAsync($"{_appOptions.WebApiUrl}/monitoredWebPages", content);
							break;
						default:
							_logger.LogError($"Job with id '{jobResult.JobId}' has unknown entity type id '{result.EntityTypeId}'");
							break;
					}

					if (response != null)
					{
						if (response.IsSuccessStatusCode)
						{
							_logger.LogInformation($"Successfully sent result for JobId: {jobResult.JobId}");
						}
						else
						{
							_logger.LogError($"Failed to send result for JobId: {jobResult.JobId}. Status Code: {response.StatusCode}");
						}
					}
				}
				else
				{
					// Wait before retrying
					_logger.LogError($"Dequieing of job results queue failed, will be retried.");
					await Task.Delay(1000);
				}
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(ResultsBackgroundService)} is stopping.");

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
