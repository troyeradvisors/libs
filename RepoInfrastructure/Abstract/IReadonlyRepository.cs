using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using System.Data.Objects.DataClasses;

namespace RepoInfrastructure.Abstract
{
	public interface IReadonlyRepository<TEntity> where TEntity:EntityObject 
	{
		IQueryable<TEntity> All { get; }
		TEntity FindBy(Expression<Func<TEntity, bool>> expression);
		IQueryable<TEntity> FilterBy(Expression<Func<TEntity, bool>> expression);
		TEntity FindBy(object key);
	}

}
