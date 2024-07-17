using JobScheduler.Plugin.WebPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMonitoring.DataAccess.Repository;
using WebMonitoring.Models;

namespace WebMonitoring.DataAccess.UnitOfWork
{
	public interface IUnitOfWork
	{
		IRepository<WebpageResult> WebpageResultRepository { get; }
		IRepository<MonitoredWebPage> MonitoredWebPageRepository { get; }
	}
}
