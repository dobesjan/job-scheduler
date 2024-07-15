using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.PluginSystem
{
	public interface IPluginManager
	{
		void RegisterPlugin(string name, IJobPlugin plugin);
		IJobPlugin GetPlugin(string name);
	}
}
