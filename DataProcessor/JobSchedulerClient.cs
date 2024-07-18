using Grpc.Net.Client;
using JobScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JobScheduler.JobSchedulerClient;

namespace DataProcessor
{
	public class JobSchedulerClient
	{
		private readonly JobSchedulerClient _client;

		public JobSchedulerClient(string address)
		{
			var channel = GrpcChannel.ForAddress(address);
			_client = new JobScheduler.JobSchedulerClient(channel);
		}

		public async Task<ScheduleJobResponse> ScheduleJob(int jobId)
		{
			throw new NotImplementedException();
		}

		public async Task<GetJobStatusResponse> GetJobStatus(int jobId)
		{
			throw new NotImplementedException();
		}

		public async Task<GetJobResponse> GetJob(string jobname, string pluginName)
		{
			throw new NotImplementedException();
		}

	}
}
