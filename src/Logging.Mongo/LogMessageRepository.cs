using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Library.Data;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Takenet.Library.Logging.Mongo
{
    /// <summary>
    /// Repository for LogMessage entity
    /// </summary>
    public class LogMessageRepository : IEntityRepository<LogMessage, Guid>
    {
        private MongoCollection<LogMessage> _collection;

        #region Constructor

        /// <summary>
        /// Creates a new repository with specific collection name
        /// </summary>
        public LogMessageRepository(string collectionName)
        {
            _collection = MongoHelper.GetCollection<LogMessage>(collectionName);
        }

        /// <summary>
        /// Creates a new repository with specific collection name and capped collection settings
        /// </summary>
        public LogMessageRepository(string collectionName, bool isCaped)
        {
            _collection = MongoHelper.GetCollection<LogMessage>(collectionName, isCaped);
        }

        /// <summary>
        /// Creates a new repository with specific collection name and capped collection settings
        /// </summary>
        public LogMessageRepository(string collectionName, bool isCaped, int collectionSize)
        {
            _collection = MongoHelper.GetCollection<LogMessage>(collectionName, isCaped, collectionSize);
        }

        /// <summary>
        /// Creates a new repository with specific collection name and capped collection settings
        /// </summary>
        public LogMessageRepository(string collectionName, bool isCaped, int collectionSize, string connectionString)
        {
            _collection = MongoHelper.GetCollection<LogMessage>(collectionName, isCaped, collectionSize, connectionString);
        }

        #endregion

        #region IEntityRepository<LogMessage,Guid> Members

        /// <summary>
        /// Adds a <typeparamref name="TEntity"/> to the repository
        /// </summary>
        /// <param name="entity">Entity instance to add on repository</param>
        /// <param name="isNew">Indicates if the entity is new or a existing value</param>
        public void Add(LogMessage entity, bool isNew)
        {
            if (isNew)
            {
                _collection.Insert<LogMessage>(entity);
            }
            else
            {
                _collection.Update(
                    Query.EQ("_id", entity.LogMessageId),
                    Update.Replace<LogMessage>(entity)
                );
            }
        }

        /// <summary>
        /// Removes a existing <typeparamref name="TEntity"/> from the repository
        /// </summary>
        /// <param name="entity">Entity instance to remove from repository</param>
        public void Remove(LogMessage entity)
        {
            _collection.Remove(Query.EQ("_id", entity.LogMessageId));
        }

        /// <summary>
        /// Gets a collection of <typeparamref name="TEntity"/> with all entities on the repository
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LogMessage> GetAll()
        {
            return _collection.FindAll().ToList();
        }

        /// <summary>
        /// Get a instance of <typeparamref name="TEntity"/> by entity key
        /// </summary>
        /// <param name="id">Entity key</param>
        /// <returns></returns>
        public LogMessage GetById(Guid id)
        {
            return _collection.Find(Query.EQ("_id", id)).FirstOrDefault();
        }

        /// <summary>
        /// Gets a generic IQueryable member
        /// </summary>
        /// <returns></returns>
        public IQueryable<LogMessage> AsQueryable()
        {
            return _collection.AsQueryable<LogMessage>();
        }

        #endregion
    }
}
