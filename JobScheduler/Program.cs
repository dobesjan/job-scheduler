using JobScheduler.Plugin.Test;
using JobScheduler;
using JobScheduler.PluginSystem;
using JobScheduler.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
	static async Task Main(string[] args)
	{
		var host = CreateHostBuilder(args).Build();

		// Initialize the database
		using (var scope = host.Services.CreateScope())
		{
			var services = scope.ServiceProvider;
			var jobStore = services.GetRequiredService<IJobStore>();
			await jobStore.InitializeDatabaseAsync();
		}

		await host.RunAsync();
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
	Host.CreateDefaultBuilder(args)
		.ConfigureServices((hostContext, services) =>
		{
			string connectionString = "Data Source=jobs.db";
			services.AddSingleton<IJobStore>(new JobStore(connectionString));
			services.AddSingleton<IPluginManager, PluginManager>(serviceProvider =>
			{
				var pluginManager = new PluginManager();
				pluginManager.RegisterPlugin(nameof(ExamplePlugin), new ExamplePlugin());
				return pluginManager;
			});
			services.AddHostedService<JobSchedulerBackgroundService>();
			services.AddGrpc();
		});

}