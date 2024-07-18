using Grpc.Core;
using JobScheduler.PluginSystem;
using JobScheduler.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler
{
	public class JobSchedulerService : JobScheduler.JobSchedulerBase
	{
		private readonly ILogger<JobSchedulerService> _logger;
		private readonly IJobStore _jobStore;
		private readonly IPluginManager _pluginManager;

		public JobSchedulerService(ILogger<JobSchedulerService> logger, IJobStore jobStore, IPluginManager pluginManager)
		{
			_logger = logger;
			_jobStore = jobStore;
			_pluginManager = pluginManager;
		}

		public override async Task<ScheduleJobResponse> ScheduleJob(ScheduleJobRequest request, ServerCallContext context)
		{
			var jobId = await _jobStore.AddJobAsync(request.JobName, request.PluginName, request.Parameters, request.Interval, request.EntityId);
			return new ScheduleJobResponse { JobId = jobId };
		}

		public override async Task<GetJobStatusResponse> GetJobStatus(GetJobStatusRequest request, ServerCallContext context)
		{
			var job = await _jobStore.GetJobAsync(request.JobId);
			return new GetJobStatusResponse { Status = job.Status, Result = job.Result };
		}

		public override async Task<GetJobResponse> GetJob(GetJobRequest request, ServerCallContext context)
		{
			var job = await _jobStore.GetJobAsync(request.EntityId);
			return new GetJobResponse { JobName = job.JobName, PluginName = job.PluginName };
		}
	}
}
