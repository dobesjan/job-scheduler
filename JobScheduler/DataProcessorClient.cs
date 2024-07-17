using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler
{
	public class DataProcessorClient
	{
		private readonly DataProcessor.DataProcessorClient _client;

		public DataProcessorClient(string address)
		{
			var channel = GrpcChannel.ForAddress(address);
			_client = new DataProcessor.DataProcessorClient(channel);
		}

		public async Task<SendResultResponse> SendResultAsync(int jobId, string result)
		{
			var request = new SendResultRequest
			{
				JobId = jobId,
				Result = result
			};

			try
			{
				var response = await _client.SendResultAsync(request);
				return response;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error sending result: {ex.Message}");
				throw;
			}
		}
	}
}
