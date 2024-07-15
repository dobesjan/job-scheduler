using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Storage
{
	public interface IJobStore
	{
		Task InitializeDatabaseAsync();
		Task<int> AddJobAsync(string jobName, string pluginName, string parameters, int interval);
		Task<Job> GetJobAsync(int jobId);
		Task<IEnumerable<Job>> GetPendingJobsAsync();
		Task<Job> GetNextJobAsync();
		Task UpdateJobStatusAsync(int jobId, string status, string result);
	}
}
