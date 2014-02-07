using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using Takenet.Library.Data;

namespace Takenet.Library.Logging.Mongo
{
    public abstract class MongoEntityRepository<TEntity, TId> : IEntityRepository<TEntity, TId> where TEntity : class
    {
        private MongoCollection<TEntity> _collection;

        #region Constructor

        public MongoEntityRepository(MongoCollection<TEntity> collection)
        {
            _collection = collection;
        }

        #endregion

        #region IEntityRepository<TEntity,TId> Members

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        public virtual void Add(TEntity entity, bool isNew)
        {
            if (isNew)
            {
                _collection.Insert(entity);
            }
            else
            {
                _collection.Save(entity);
            }
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Remove(TEntity entity)
        {            
            _collection.Remove(Query.EQ("_id", GetEntityId(entity).ToBson()));
        }

        /// <summary>
        /// Gets all entities in the repository
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return _collection.FindAll().ToList();
        }

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public virtual TEntity GetById(TId id)
        {
            return _collection.Find(Query.EQ("_id", id.ToBson())).FirstOrDefault();
        }

        /// <summary>
        /// Gets a generic IQueryable member
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> AsQueryable()
        {
            return _collection.AsQueryable<TEntity>();
        }

        #endregion

        protected abstract TId GetEntityId(TEntity entity);

    }
}
