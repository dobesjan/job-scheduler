using Grpc.Core;
using Grpc.Core.Api;
using JobScheduler.Storage;
using Moq;
using static Moq.It;

namespace JobScheduler.IntegrationTests
{
	/*
	public class JobSchedulerIntegrationTests
	{
		private readonly Mock<JobScheduler.JobSchedulerClient> _jobSchedulerClientMock;
		private readonly string _connectionString = $"Data Source={Guid.NewGuid()}.db";

		public JobSchedulerIntegrationTests()
		{
			_jobSchedulerClientMock = new Mock<JobScheduler.JobSchedulerClient>();
		}

		private async Task<JobStore> InitializeJobStoreAsync()
		{
			var connectionString = $"Data Source={Guid.NewGuid()}.db";
			var jobStore = new JobStore(connectionString);
			await jobStore.InitializeDatabaseAsync();

			return jobStore;
		}

		[Fact]
		public async Task ScheduleJob_ShouldInsertJobIntoDatabase()
		{
			using var jobStore = await InitializeJobStoreAsync();
			var scheduleJobRequest = new ScheduleJobRequest
			{
				JobName = "TestJob",
				PluginName = "TestPlugin",
				Parameters = "{}",
				Interval = 60
			};
			var scheduleJobResponse = new ScheduleJobResponse { JobId = 1 };

			_jobSchedulerClientMock
			.Setup(client => client.ScheduleJobAsync(
				It.IsAny<ScheduleJobRequest>(),
				It.IsAny<CallOptions>()))
			.ReturnsAsync(CreateAsyncUnaryCall(scheduleJobResponse));

			var client = _jobSchedulerClientMock.Object;

			var response = await client.ScheduleJobAsync(scheduleJobRequest);
			var jobId = response.JobId;

			var job = await jobStore.GetJobAsync(jobId);
			Assert.NotNull(job);
			Assert.Equal("TestJob", job.JobName);
			Assert.Equal("TestPlugin", job.PluginName);
			Assert.Equal("{}", job.Parameters);
			Assert.Equal(60, job.Interval);
		}

		[Fact]
		public async Task GetJobStatus_ShouldReturnCorrectStatus()
		{
			using var jobStore = await InitializeJobStoreAsync();
			var jobId = await jobStore.AddJobAsync("TestJob", "TestPlugin", "{}", 60);
			await jobStore.UpdateJobStatusAsync(jobId, "Completed", "Success");

			var getJobStatusRequest = new GetJobStatusRequest { JobId = jobId };
			var getJobStatusResponse = new GetJobStatusResponse
			{
				Status = "Completed",
				Result = "Success"
			};

			_jobSchedulerClientMock
			.Setup(client => client.GetJobStatusAsync(
				It.IsAny<GetJobStatusRequest>(),
				It.IsAny<CallOptions>()))
			.ReturnsAsync(getJobStatusResponse);

			var client = _jobSchedulerClientMock.Object;

			var response = await client.GetJobStatusAsync(getJobStatusRequest);

			Assert.Equal("Completed", response.Status);
			Assert.Equal("Success", response.Result);
		}
	}
	*/
}