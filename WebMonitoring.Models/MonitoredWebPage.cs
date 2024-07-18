using JobScheduler.Plugin.WebPage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;

namespace WebMonitoring.Models
{
	public class MonitoredWebPage : IMonitoredEntity
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

		public int Interval { get; set; }

		[NotMapped]
		[ValidateNever]
		public string Parameters
		{
			get
			{
				var config = new WebpageConfig
				{
					Url = Url,
					EntityId = EntityId
				};

				return JsonSerializer.Serialize(config);
			}
		}

		public bool IsEntityTypeOf(EMonitoredEntityType entityType)
		{
			return MonitoredEntityTypeId == (int)entityType;
		}
	}
}
