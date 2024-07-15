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
		Task<int> AddJobAsync(string jobName, string pluginName, string parameters);
		Task<Job> GetJobAsync(string jobId);
	}
}
