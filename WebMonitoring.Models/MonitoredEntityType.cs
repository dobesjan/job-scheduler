using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMonitoring.Models
{
	public enum EMonitoredEntityType
	{
		WebPage = 1
	}

	public class MonitoredEntityType
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }
	}
}
