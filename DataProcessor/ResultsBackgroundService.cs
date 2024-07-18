using DataProcessor.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor
{
	// communication from scheduler to api
	public class ResultsBackgroundService : IHostedService, IDisposable
	{
		private readonly ILogger<ResultsBackgroundService> _logger;
		private readonly IServiceProvider _serviceProvider;
		private Timer _timer;
		private CancellationTokenSource _cancellationTokenSource;
		private readonly ConcurrentQueue<(int JobId, string Result)> _jobResultsQueue;
		private Task _resultProcessorTask;
		private readonly AppOptions _appOptions;
		private readonly HttpClient _httpClient;

		public ResultsBackgroundService(ILogger<ResultsBackgroundService> logger, IServiceProvider serviceProvider,  IOptions<AppOptions> options)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_jobResultsQueue = new ConcurrentQueue<(int JobId, string Result)>();
			_appOptions = options.Value;
			_httpClient = new HttpClient();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			// Receive results from queue created by DataProcessorService
			// Consider data decoration in future
			// Send to webapi which will store this data to db


			throw new NotImplementedException();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			_httpClient.Dispose();
		}
	}
}
