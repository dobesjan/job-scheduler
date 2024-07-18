using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMonitoring.Models
{
	public interface IMonitoredEntity
	{
		public int Id { get; set; }
		public string EntityId { get; set; }

		public int MonitoredEntityTypeId { get; set; }

		public int Interval { get; set; }

		public string Parameters { get; }
	}
}
