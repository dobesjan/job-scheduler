using Grpc.Core;
using JobScheduler;
using System;
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

		public override async Task<SendResultResponse> SendResult(SendResultRequest request, ServerCallContext context)
		{
			// receive results from JobScheduler and paste them to queue

			throw new NotImplementedException();
		}
	}
}
