using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Storage
{
	public class JobStore : IJobStore, IDisposable
	{
		private readonly string _connectionString;
		private SqliteConnection _connection;

		public JobStore(string connectionString)
		{
			_connectionString = connectionString;
			_connection = CreateConnection();
		}

		private SqliteConnection CreateConnection()
		{
			var connection = new SqliteConnection(_connectionString);
			connection.Open();
			return connection;
		}

		public SqliteConnection GetConnection()
		{
			if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
			{
				_connection = CreateConnection();
			}
			return _connection;
		}

		public async Task InitializeDatabaseAsync()
		{
			using var connection = GetConnection();
			await connection.ExecuteAsync(
				@"CREATE TABLE IF NOT EXISTS Jobs (
                JobId INTEGER PRIMARY KEY AUTOINCREMENT,
				EntityId TEXT NOT NULL,
                JobName TEXT NOT NULL,
                PluginName TEXT NOT NULL,
                Parameters TEXT,
                Status TEXT,
                Result TEXT,
				Interval INTEGER,
				NextExecution INTEGER
              );");
		}

		public async Task<int> AddJobAsync(string jobName, string pluginName, string parameters, int interval, string entityId)
		{
			using var connection = GetConnection();
			var nextExecution = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + interval;
			var jobId = await connection.ExecuteScalarAsync<int>(
				"INSERT INTO Jobs (JobName, PluginName, Parameters, Status, Interval, NextExecution, EntityId) VALUES (@JobName, @PluginName, @Parameters, 'Scheduled', @Interval, @NextExecution, @EntityId); SELECT last_insert_rowid();",
				new { JobName = jobName, PluginName = pluginName, Parameters = parameters, Interval = interval, NextExecution = nextExecution, EntityId = entityId });
			return jobId;
		}

		public async Task<Job> GetJobAsync(int jobId)
		{
			using var connection = GetConnection();
			return await connection.QueryFirstOrDefaultAsync<Job>("SELECT * FROM Jobs WHERE JobId = @JobId", new { JobId = jobId });
		}

		public async Task<Job> GetJobAsync(string entityId)
		{
			using var connection = GetConnection();
			return await connection.QueryFirstOrDefaultAsync<Job>("SELECT * FROM Jobs WHERE EntityId = @EntityId", new { EntityId = entityId });
		}

		public async Task<IEnumerable<Job>> GetPendingJobsAsync()
		{
			using var connection = GetConnection();
			return await connection.QueryAsync<Job>("SELECT * FROM Jobs WHERE Status = 'Scheduled' AND NextExecution <= @CurrentTimestamp", new { CurrentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
		}

		public async Task<Job> GetNextJobAsync()
		{
			using var connection = GetConnection();
			// TODO: Consider priority
			return await connection.QueryFirstOrDefaultAsync<Job>("SELECT * FROM Jobs WHERE Status = 'Scheduled' AND NextExecution <= @CurrentTimestamp ORDER BY NextExecution DESC", new { CurrentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
		}

		public async Task UpdateJobStatusAsync(int jobId, string status, string result)
		{
			using var connection = GetConnection();
			var job = await GetJobAsync(jobId);
			var nextExecution = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + job.Interval;
			await connection.ExecuteAsync(
				"UPDATE Jobs SET Status = @Status, Result = @Result, NextExecution = @NextExecution WHERE JobId = @JobId",
				new { Status = status, Result = result, JobId = jobId, NextExecution = nextExecution });
		}

		public void Dispose()
		{
			_connection?.Close();
			_connection?.Dispose();
		}
	}
}
