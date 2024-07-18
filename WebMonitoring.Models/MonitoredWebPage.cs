using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMonitoring.Models
{
	public class MonitoredWebPage
	{
		[Key]
		public int Id { get; set; }
		public string EntityId { get; set; } = Guid.NewGuid().ToString();

		public int MonitoredEntityTypeId { get; set; }

		[ForeignKey(nameof(MonitoredEntityTypeId))]
		[ValidateNever]
		public MonitoredEntityType MonitoredEntityType { get; set; }

		public string Name { get; set; }
		public string Url { get; set; }

		public bool IsEntityTypeOf(EMonitoredEntityType entityType)
		{
			return MonitoredEntityTypeId == (int)entityType;
		}
	}
}
