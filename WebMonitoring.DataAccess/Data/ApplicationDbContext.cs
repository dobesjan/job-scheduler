﻿using JobScheduler.Plugin.WebPage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMonitoring.Models;

namespace WebMonitoring.DataAccess.Data
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<MonitoredWebPage> WebPages { get; set; }
		public DbSet<WebpageResult> WebpageMetrics { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
	}
}
