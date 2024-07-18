using DataProcessor.Models;
using Grpc.Core;
using JobScheduler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static JobScheduler.DataProcessor;

namespace DataProcessor
{
	public class DataProcessorService : DataProcessorBase
	{
		// This service is receiving results from JobScheduler

		private readonly ConcurrentQueue<ProcessedResult> _jobResultsQueue;

		public DataProcessorService(ConcurrentQueue<ProcessedResult> jobResultsQueue)
		{
			_jobResultsQueue = jobResultsQueue;
		}

		public override async Task<SendResultResponse> SendResult(SendResultRequest request, ServerCallContext context)
		{
			var result = new ProcessedResult
			{
				JobId = request.JobId,
				Result = request.Result
			};

			_jobResultsQueue.Enqueue(result);

			return new SendResultResponse { JobId = result.JobId, Result = result.Result };
		}
	}
}
