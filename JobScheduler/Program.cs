using JobScheduler.Plugin.Test;
using JobScheduler;
using JobScheduler.PluginSystem;
using JobScheduler.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JobScheduler.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

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
		.ConfigureAppConfiguration((hostingContext, config) =>
		{
			config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
		})
		.ConfigureServices((hostContext, services) =>
		{
			services.Configure<AppOptions>(hostContext.Configuration.GetSection("App"));
			var appOptions = hostContext.Configuration.GetSection("App").Get<AppOptions>();

			string connectionString = appOptions.DefaultConnection;
			services.AddSingleton<IJobStore>(new JobStore(connectionString));
			services.AddSingleton<IPluginManager, PluginManager>(serviceProvider =>
			{
				var pluginManager = new PluginManager();
				pluginManager.RegisterPlugin(nameof(ExamplePlugin), new ExamplePlugin());
				return pluginManager;
			});
			services.AddHostedService<JobSchedulerBackgroundService>();
			services.AddGrpc();

			string clientUrl = appOptions.DataProcessorUrl;
			services.AddGrpcClient<DataProcessor.DataProcessorClient>(options =>
			{
				options.Address = new Uri(clientUrl);
			});

			// Register DataProcessorClient as a singleton
			services.AddSingleton<DataProcessorClient>();
			
		});

}