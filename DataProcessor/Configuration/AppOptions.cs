using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Configuration
{
	public class AppOptions
	{
		public string WebApiUrl { get; set; }
		public string JobSchedulerUrl { get; set; }
		public int Interval { get; set; }
	}
}
