using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebMonitoring.DataAccess.Data;

namespace WebMonitoring.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		private readonly string _properties;
		internal DbSet<T> dbset;

		public Repository(ApplicationDbContext db, string properties = "")
		{
			_db = db;
			this.dbset = _db.Set<T>();
			_properties = properties;
		}

		public T Add(T entity)
		{
			dbset.Add(entity);

			return entity;
		}

		public T Update(T entity)
		{
			_db.ChangeTracker.Clear();
			dbset.Update(entity);

			return entity;
		}

		public void Detach(T entity)
		{
			var entry = dbset.Entry(entity);
			if (entry.State != EntityState.Detached)
			{
				entry.State = EntityState.Detached;
			}
		}

		public T Get(Expression<Func<T, bool>> filter, bool tracked = false)
		{
			IQueryable<T> query = GetInternal(_properties, tracked);
			return query.Where(filter).FirstOrDefault();
		}

		private IQueryable<T> GetInternal(string? includeProperties, bool tracked)
		{
			IQueryable<T> query;

			if (tracked)
			{
				query = dbset;
			}
			else
			{
				//Db will not be automatically updated on entity change
				query = dbset.AsNoTrackingWithIdentityResolution();
			}

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProp in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}

			return query;
		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, int offset = 0, int limit = 0)
		{
			IQueryable<T> query = dbset.AsNoTrackingWithIdentityResolution();
			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (!string.IsNullOrEmpty(_properties))
			{
				foreach (var includeProp in _properties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}

			if (limit > 0)
			{
				query = query.Skip(offset).Take(limit);
			}

			return query.ToList();
		}

		public int Count(Expression<Func<T, bool>>? filter = null)
		{
			if (filter != null)
			{
				return dbset.Count(filter);
			}
			return dbset.Count();
		}

		public void Remove(T entity)
		{
			dbset.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entity)
		{
			dbset.RemoveRange(entity);
		}

		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
