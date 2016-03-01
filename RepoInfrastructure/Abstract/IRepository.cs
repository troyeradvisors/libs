using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace RepoInfrastructure.Abstract
{
	public interface IRepository<TEntity> : IReadonlyRepository<TEntity> where TEntity : EntityObject
	{
		void Add(TEntity entity, bool save);
        TEntity Update(TEntity entity, bool save);
		bool Save();
		void Delete(TEntity entity, bool save);
	}
}
