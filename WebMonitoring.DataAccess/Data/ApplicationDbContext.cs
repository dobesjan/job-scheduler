using JobScheduler.Plugin.WebPage;
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
	public static class CompiledModels
	{
		public static readonly IModel ApplicationDbContextModel = BuildModel();

		private static IModel BuildModel()
		{
			var builder = new ModelBuilder();
			ConfigureModel(builder);
			return builder.FinalizeModel();
		}

		private static void ConfigureModel(ModelBuilder builder)
		{
			builder.Entity<MonitoredWebPage>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Url).IsRequired();
				entity.Property(e => e.Name).IsRequired();
			});

			builder.Entity<WebpageResult>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Url).IsRequired();
				entity.Property(e => e.ResponseTime).IsRequired();
				entity.Property(e => e.StatusCode).IsRequired();
				entity.Property(e => e.ContentLength).IsRequired();
				entity.Property(e => e.ErrorMessage).IsRequired();
			});
		}
	}

	public class ApplicationDbContext : DbContext
	{
		public DbSet<MonitoredWebPage> WebPages { get; set; }
		public DbSet<WebpageResult> WebpageMetrics { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.UseModel(CompiledModels.ApplicationDbContextModel);
		}
	}
}
