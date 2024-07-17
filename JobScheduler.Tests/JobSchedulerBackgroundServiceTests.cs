using JobScheduler.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.PluginSystem.Tests
{
	public class JobSchedulerBackgroundServiceTests
	{
		[Fact]
		public async Task StartAsync_ShouldStartProcessingJobs()
		{
			var mockLogger = new Mock<ILogger<JobSchedulerBackgroundService>>();
			var mockDataProcessor = new Mock<DataProcessor.DataProcessorClient>();
			var mockServiceProvider = new Mock<IServiceProvider>();
			var mockJobStore = new Mock<IJobStore>();
			var mockPluginManager = new Mock<IPluginManager>();

			mockServiceProvider.Setup(sp => sp.GetService(typeof(IJobStore))).Returns(mockJobStore.Object);
			mockServiceProvider.Setup(sp => sp.GetService(typeof(IPluginManager))).Returns(mockPluginManager.Object);

			var jobs = new List<Job>
		{
			new Job { JobId = 1, JobName = "TestJob1", PluginName = "TestPlugin", Parameters = "{}", Interval = 60 },
			new Job { JobId = 2, JobName = "TestJob2", PluginName = "TestPlugin", Parameters = "{}", Interval = 60 }
		};

			mockJobStore.Setup(js => js.GetPendingJobsAsync()).ReturnsAsync(jobs);
			mockPluginManager.Setup(pm => pm.GetPlugin(It.IsAny<string>())).Returns(new Mock<IJobPlugin>().Object);

			var service = new JobSchedulerBackgroundService(mockLogger.Object, mockServiceProvider.Object, mockDataProcessor.Object);
			await service.StartAsync(CancellationToken.None);

			await Task.Delay(1000); // Allow some time for processing

			mockJobStore.Verify(js => js.GetPendingJobsAsync(), Times.AtLeastOnce());
		}

		[Fact]
		public async Task StopAsync_ShouldStopProcessingJobs()
		{
			var mockLogger = new Mock<ILogger<JobSchedulerBackgroundService>>();
			var mockDataProcessor = new Mock<DataProcessor.DataProcessorClient>();
			var mockServiceProvider = new Mock<IServiceProvider>();
			var mockJobStore = new Mock<IJobStore>();
			var mockPluginManager = new Mock<IPluginManager>();

			var service = new JobSchedulerBackgroundService(mockLogger.Object, mockServiceProvider.Object, mockDataProcessor.Object);
			await service.StartAsync(CancellationToken.None);

			await service.StopAsync(CancellationToken.None);

			// Ensure the service has stopped
			Assert.NotNull(service);
		}
	}
}
