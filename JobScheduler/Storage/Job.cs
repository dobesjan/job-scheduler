using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Storage
{
	public class Job
	{
		public int JobId { get; set; }
		public string JobName { get; set; }
		public string PluginName { get; set; }
		public string Parameters { get; set; }
		public string Status { get; set; }
		public string Result { get; set; }
		public int Interval { get; set; }
		public int NextExecution { get; set; }
	}
}
