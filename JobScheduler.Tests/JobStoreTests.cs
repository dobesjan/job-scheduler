using Dapper;
using JobScheduler.Storage;
using Microsoft.Data.Sqlite;

namespace JobScheduler.Tests
{
	public class JobStoreTests
	{
		private async Task<JobStore> InitializeJobStoreAsync()
		{
			var connectionString = $"Data Source={Guid.NewGuid()}.db";
			var jobStore = new JobStore(connectionString);
			await jobStore.InitializeDatabaseAsync();

			return jobStore;
		}

		[Fact]
		public async Task InitializeDatabaseAsync_ShouldCreateJobsTable()
		{
			using var jobStore = await InitializeJobStoreAsync();

			var result = await jobStore.GetConnection().ExecuteScalarAsync<int>(
				"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Jobs'");

			Assert.Equal(1, result);
		}

		[Fact]
		public async Task AddJobAsync_ShouldAddJob()
		{
			using var jobStore = await InitializeJobStoreAsync();

			var jobId = await jobStore.AddJobAsync("TestJob", "TestPlugin", "{}", 60);

			var job = await jobStore.GetJobAsync(jobId);

			Assert.NotNull(job);
			Assert.Equal("TestJob", job.JobName);
			Assert.Equal("TestPlugin", job.PluginName);
			Assert.Equal("{}", job.Parameters);
			Assert.Equal(60, job.Interval);
		}

		[Fact]
		public async Task GetJobAsync_ShouldReturnCorrectJob()
		{
			using var jobStore = await InitializeJobStoreAsync();

			var jobId = await jobStore.AddJobAsync("TestJob", "TestPlugin", "{}", 60);
			var job = await jobStore.GetJobAsync(jobId);

			Assert.NotNull(job);
			Assert.Equal(jobId, job.JobId);
			Assert.Equal("TestJob", job.JobName);
			Assert.Equal("TestPlugin", job.PluginName);
		}

		[Fact]
		public async Task GetPendingJobsAsync_ShouldReturnPendingJobs()
		{
			using var jobStore = await InitializeJobStoreAsync();

			await jobStore.AddJobAsync("TestJob1", "TestPlugin1", "{}", 0);
			await jobStore.AddJobAsync("TestJob2", "TestPlugin2", "{}", 0);

			var jobs = await jobStore.GetPendingJobsAsync();

			Assert.Equal(2, jobs.Count());
		}

		[Fact]
		public async Task GetNextJobAsync_ShouldReturnNextPendingJob()
		{
			using var jobStore = await InitializeJobStoreAsync();

			var jobId1 = await jobStore.AddJobAsync("TestJob1", "TestPlugin1", "{}", 100);
			await Task.Delay(1000); // Ensure a different NextExecution timestamp
			var jobId2 = await jobStore.AddJobAsync("TestJob2", "TestPlugin2", "{}", 0);
			var jobId3 = await jobStore.AddJobAsync("TestJob3", "TestPlugin3", "{}", 500);

			var nextJob = await jobStore.GetNextJobAsync();

			Assert.NotNull(nextJob);
			Assert.Equal(jobId2, nextJob.JobId); // Should return the first job added
		}

		[Fact]
		public async Task UpdateJobStatusAsync_ShouldUpdateStatusAndNextExecution()
		{
			using var jobStore = await InitializeJobStoreAsync();

			var jobId = await jobStore.AddJobAsync("TestJob", "TestPlugin", "{}", 60);
			await jobStore.UpdateJobStatusAsync(jobId, "Completed", "Success");

			var job = await jobStore.GetJobAsync(jobId);

			Assert.Equal("Completed", job.Status);
			Assert.Equal("Success", job.Result);
			Assert.True(job.NextExecution > DateTimeOffset.UtcNow.ToUnixTimeSeconds());
		}
	}
}