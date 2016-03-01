using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data;
using System.Data.Objects.DataClasses;

using RepoInfrastructure.Abstract;

namespace RepoInfrastructure.Concrete
{
	public class ContextRepository<T, TContext> : ContextReadonlyRepository<T, TContext>,  IRepository<T> 
		where T : EntityObject
		where TContext : ObjectContext
	{
        public ContextRepository(bool enableLazyLoading = true) : base(enableLazyLoading) { }

		public void Add(T entity, bool save = true)
		{
            if (entity.EntityState == EntityState.Detached)
			    Context.AddObject(EntitySetName, entity);
			if (save) Save();
		}

        /// <summary>
        /// Adds or updates as needed.
        /// </summary>
        /// <returns>Updated or added entity in context manager</returns>
        public T Update(T entity, bool save = true)
        {
            bool created;
            return Update(entity, out created, save);
        }

        public T Update(T entity, out bool created, bool save = true)
        {
            T updated = null;
            if (IsNew(entity))
            {
                Context.AddObject(EntitySetName, entity);
                created = true;
                updated = entity;
            }
            else
            {
                object originalItem = null;
                // Create the detached object's entity key. 
                //EntityKey key = Context.CreateEntityKey(EntitySetName, entity);
                // Get the original item based on the entity key from the context 
                // or from the database. 
                if (Context.TryGetObjectByKey(entity.EntityKey, out originalItem))
                {
                    // Call the ApplyCurrentValues method to apply changes 
                    // from the updated item to the original version. 
                    Context.ApplyCurrentValues(entity.EntityKey.EntitySetName, entity);
                    created = false;
                    updated = (T)originalItem;
                }
                else
                {
                    Context.AddObject(EntitySetName, entity);
                    created = true;
                    updated = entity;
                }
            }
            if (save) Save();
            return updated;
        }



        protected EntityKey EntityKey(EntityObject o)
        {
            if (o.EntityKey == null)
                o.EntityKey = Context.CreateEntityKey(EntitySetName, o);
            return o.EntityKey;
        }

        /// <summary>
        /// Returns true if entitykey contains any default values.  
        /// </summary>
        public bool IsNew(EntityObject o)
        {
            EntityKey(o);
            return o.EntityKey.EntityKeyValues.Any(keyValue =>
            {
                Type t = keyValue.GetType();
                return keyValue == null || t.IsValueType && Equals(defaultValues[t] ?? (defaultValues[t] = Activator.CreateInstance(t)), keyValue);
            });
        }

        Dictionary<Type, object> defaultValues = new Dictionary<Type, object>();


		public bool Save()
		{
			return Context.SaveChanges() > 0;
		}

		public void Delete(T entity, bool save = true)
		{
			Context.AttachTo(EntitySetName, entity);
			Context.DeleteObject(entity);
			if (save)
				Save();
		}

        /*
        public void Update(T entity, bool save = true)
        {
            Context.AttachTo(EntitySetName, entity);
            //if (Context.ObjectStateManager.
            Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
			if (save)
				Save();
        }*/
    }
}
