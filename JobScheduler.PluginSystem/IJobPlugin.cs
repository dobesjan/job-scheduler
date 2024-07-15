using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.PluginSystem
{
	public interface IJobPlugin
	{
		Task<string> ExecuteAsync(string parameters);
	}
}
