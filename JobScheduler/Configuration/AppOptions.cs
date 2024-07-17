using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Configuration
{
	public class AppOptions
	{
		public string DefaultConnection { get; set; }
		public int Interval { get; set; }
		public string DataProcessorUrl { get; set; }
	}
}
