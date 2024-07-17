using JobScheduler.Plugin.WebPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMonitoring.DataAccess.Data;
using WebMonitoring.DataAccess.Repository;
using WebMonitoring.Models;

namespace WebMonitoring.DataAccess.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private ApplicationDbContext _context;

		public IRepository<WebpageResult> WebpageResultRepository { get; }
		public IRepository<MonitoredWebPage> MonitoredWebPageRepository { get; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			WebpageResultRepository = new Repository<WebpageResult>(context);
			MonitoredWebPageRepository = new Repository<MonitoredWebPage>(context);
		}
	}
}
