using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Plugin.WebPage
{
	public class WebpageResult
	{
		public Guid Id { get; set; }
		public string Url { get; set; }
		public long ResponseTime { get; set; } // in milliseconds
		public int StatusCode { get; set; }
		public long ContentLength { get; set; }
		public string ErrorMessage { get; set; }
	}
}
