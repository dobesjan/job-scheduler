using Moq;

namespace JobScheduler.PluginSystem.Tests
{
	public class PluginManagerTests
	{
		[Fact]
		public void GetPlugin_ShouldReturnPlugin()
		{
			var mockPlugin = new Mock<IJobPlugin>();
			var pluginManager = new PluginManager();
			pluginManager.RegisterPlugin("TestPlugin", mockPlugin.Object);

			var plugin = pluginManager.GetPlugin("TestPlugin");

			Assert.NotNull(plugin);
		}

		[Fact]
		public void GetPlugin_ShouldReturnNullForUnknownPlugin()
		{
			var pluginManager = new PluginManager();

			var plugin = pluginManager.GetPlugin("UnknownPlugin");

			Assert.Null(plugin);
		}
	}
}