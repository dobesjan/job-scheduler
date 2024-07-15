using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.PluginSystem
{
	//TODO: Consider if should i use IOC container instead of dictionary
	public class PluginManager : IPluginManager
	{
		private readonly Dictionary<string, IJobPlugin> _plugins;

		public PluginManager()
		{
			_plugins = new Dictionary<string, IJobPlugin>();
		}

		public void RegisterPlugin(string name, IJobPlugin plugin)
		{
			_plugins[name] = plugin;
		}

		public IJobPlugin GetPlugin(string name)
		{
			return _plugins.TryGetValue(name, out var plugin) ? plugin : null;
		}
	}
}
