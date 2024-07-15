using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Storage
{
	public class JobStore : IJobStore
	{
		private readonly string _connectionString;

		public JobStore(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task InitializeDatabaseAsync()
		{
			using var connection = new SqliteConnection(_connectionString);
			await connection.ExecuteAsync(
				@"CREATE TABLE IF NOT EXISTS Jobs (
                JobId INTEGER PRIMARY KEY AUTOINCREMENT,
                JobName TEXT NOT NULL,
                PluginName TEXT NOT NULL,
                Parameters TEXT,
                Status TEXT,
                Result TEXT
              );");
		}

		public async Task<int> AddJobAsync(string jobName, string pluginName, string parameters)
		{
			using var connection = new SqliteConnection(_connectionString);
			var jobId = await connection.ExecuteScalarAsync<int>(
				"INSERT INTO Jobs (JobName, PluginName, Parameters, Status) VALUES (@JobName, @PluginName, @Parameters, 'Scheduled'); SELECT last_insert_rowid();",
				new { JobName = jobName, PluginName = pluginName, Parameters = parameters });
			return jobId;
		}

		public async Task<Job> GetJobAsync(string jobId)
		{
			using var connection = new SqliteConnection(_connectionString);
			return await connection.QueryFirstOrDefaultAsync<Job>("SELECT * FROM Jobs WHERE JobId = @JobId", new { JobId = jobId });
		}
	}
}
