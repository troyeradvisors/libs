using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Objects;
using System.Data;
using RepoInfrastructure.Abstract;
using System.Data.Objects.DataClasses;

namespace RepoInfrastructure.Concrete
{
	public class ContextReadonlyRepository<T, TContext> : IReadonlyRepository<T>
		where T : EntityObject
		where TContext : ObjectContext
	{


		protected readonly TContext Context = FastActivator.Create<TContext>();
		private readonly string KeyProperty;
        protected string EntitySetName { get { return Context.DefaultContainerName + "." + typeof(T).Name + "s"; } }



		public ContextReadonlyRepository(bool lazyLoadingEnabled = true)
		{
            Context.ContextOptions.LazyLoadingEnabled = lazyLoadingEnabled;
			KeyProperty = Context.CreateObjectSet<T>().EntitySet.ElementType.KeyMembers[0].ToString();
		}

		public IQueryable<T> All
		{
			get
			{
				return Context.CreateObjectSet<T>();
			}
		}

		public IQueryable<T> AllWith(params string[] includes)
		{
			ObjectQuery<T> q = Context.CreateObjectSet<T>();
			foreach (string s in includes)
				q = q.Include(s);
			return q;
		}

		public T FindBy(Expression<System.Func<T, bool>> expression)
		{
			return FilterBy(expression).Single();
		}

		public IQueryable<T> FilterBy(Expression<System.Func<T, bool>> expression)
		{
			return All.Where(expression);
		}



		public T FindBy(object id)
		{
			if (id == null)
				return null;
			Context.CreateObjectSet<T>();
            EntityKey key = new EntityKey(EntitySetName, new[] { new EntityKeyMember(KeyProperty, id) });
			return (T)Context.GetObjectByKey(key);
		}
	}
}