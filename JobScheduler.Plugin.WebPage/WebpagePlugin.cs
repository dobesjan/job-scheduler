using JobScheduler.PluginSystem;
using Newtonsoft.Json;
using System.Diagnostics;

namespace JobScheduler.Plugin.WebPage
{
	public class WebpagePlugin : IJobPlugin
	{
		public async Task<string> ExecuteAsync(string parameters)
		{
			var config = JsonConvert.DeserializeObject<WebpageConfig>(parameters);
			var url = config.Url;

			using (var httpClient = new HttpClient())
			{
				try
				{
					var stopwatch = Stopwatch.StartNew();

					var response = await httpClient.GetAsync(url);

					stopwatch.Stop();

					var responseTime = stopwatch.ElapsedMilliseconds;
					var statusCode = (int)response.StatusCode;
					var contentLength = response.Content.Headers.ContentLength ?? 0;

					var result = new WebpageResult
					{
						Id = Guid.NewGuid(),
						EntityId = config.EntityId,
						Url = url,
						DateTime = DateTime.UtcNow,
						ResponseTime = responseTime,
						StatusCode = statusCode,
						ContentLength = contentLength,
						HasError = false
					};

					return JsonConvert.SerializeObject(result);
				}
				catch (Exception ex)
				{
					var errorResult = new WebpageResult
					{
						Id = Guid.NewGuid(),
						EntityId = config.EntityId,
						DateTime = DateTime.UtcNow,
						Url = url,
						HasError = true,
						ErrorMessage = ex.Message
					};
					return JsonConvert.SerializeObject(errorResult);
				}
			}
		}
	}
}
