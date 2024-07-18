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
	// communication from web api to job scheduler
	public class DataProcessorBackgroundService : IHostedService, IDisposable
	{
		private readonly ILogger<DataProcessorBackgroundService> _logger;
		private readonly IServiceProvider _serviceProvider;
		private Timer _timer;
		private CancellationTokenSource _cancellationTokenSource;
		private readonly ConcurrentQueue<(int JobId, string Result)> _jobResultsQueue;
		private Task _resultProcessorTask;
		private readonly AppOptions _appOptions;

		public DataProcessorBackgroundService(ILogger<DataProcessorBackgroundService> logger, IServiceProvider serviceProvider, IOptions<AppOptions> options)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_jobResultsQueue = new ConcurrentQueue<(int JobId, string Result)>();
			_appOptions = options.Value;
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			//TODO: Make web api call and get all entities to monitor

			// It's up to consider how we can resolve these entities, we should compare all entities and check if jobs are properly 
			// scheduled in JobScheduler (if not then schedule job with corresponding plugin)

			throw new NotImplementedException();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
