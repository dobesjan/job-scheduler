using JobScheduler.PluginSystem;

namespace JobScheduler.Plugin.Test
{
	public class ExamplePlugin : IJobPlugin
	{
		public async Task<string> ExecuteAsync(string parameters)
		{
			// Simulate some work
			await Task.Delay(1000);
			return $"Processed {parameters}";
		}
	}
}
