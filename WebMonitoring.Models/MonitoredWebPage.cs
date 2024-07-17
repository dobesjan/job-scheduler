﻿using System.ComponentModel.DataAnnotations;

namespace WebMonitoring.Models
{
	public class MonitoredWebPage
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
	}
}
