using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WebMonitoring.DataAccess.Repository
{
	public interface IRepository<T> where T : class
	{
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, int offset = 0, int limit = 0);
		int Count(Expression<Func<T, bool>>? filter = null);
		T Get(Expression<Func<T, bool>> filter, bool tracked = false);
		T Add(T entity);
		T Update(T entity);
		void Detach(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entity);
		void Save();
	}
}
