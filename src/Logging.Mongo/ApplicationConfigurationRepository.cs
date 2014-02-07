using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Library.Logging.Models;
using Takenet.Library.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using System.Configuration;

namespace Takenet.Library.Logging.Mongo
{
    /// <summary>
    /// Repository for LogMessage entity
    /// </summary>
    public class ApplicationConfigurationRepository : IEntityRepository<ApplicationConfiguration, Guid>
    {
        public const string COLLECTION_NAME = "ApplicationConfigurations";
        private MongoCollection<ApplicationConfiguration> _collection;

        #region Constructor

        /// <summary>
        /// Creates a new repository with default collection name
        /// </summary>
        public ApplicationConfigurationRepository()
            : this(COLLECTION_NAME, ConfigurationManager.ConnectionStrings[MongoHelper.DEFAULT_MONGODB_CONNECTIONSTRING_NAME].ConnectionString)
        {
        }

        /// <summary>
        /// Creates a new repository with default collection name
        /// </summary>
        public ApplicationConfigurationRepository(string connectionString)
            : this(COLLECTION_NAME, connectionString)
        {

        }

        /// <summary>
        /// Creates a new repository with specific collection name
        /// </summary>
        public ApplicationConfigurationRepository(string collectionName, string connectionString)
        {            
            _collection = MongoHelper.GetCollection<ApplicationConfiguration>(collectionName, false, 0, connectionString);
        }

        #endregion

        #region IEntityRepository<ApplicationConfiguration,Guid> Members

        /// <summary>
        /// Adds a <typeparamref name="TEntity"/> to the repository
        /// </summary>
        /// <param name="entity">Entity instance to add on repository</param>
        /// <param name="isNew">Indicates if the entity is new or a existing value</param>
        public void Add(ApplicationConfiguration entity, bool isNew)
        {
            if (isNew)
            {
                _collection.Insert<ApplicationConfiguration>(entity);
            }
            else
            {
                _collection.Update(
                    Query.EQ("_id", entity.ApplicationConfigurationId),
                    Update.Replace<ApplicationConfiguration>(entity)
                );
            }
        }

        /// <summary>
        /// Removes a existing <typeparamref name="TEntity"/> from the repository
        /// </summary>
        /// <param name="entity">Entity instance to remove from repository</param>
        public void Remove(ApplicationConfiguration entity)
        {
            _collection.Remove(Query.EQ("_id", entity.ApplicationConfigurationId));
        }

        /// <summary>
        /// Gets a collection of <typeparamref name="TEntity"/> with all entities on the repository
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ApplicationConfiguration> GetAll()
        {
            return _collection.FindAll().ToList();
        }

        /// <summary>
        /// Get a instance of <typeparamref name="TEntity"/> by entity key
        /// </summary>
        /// <param name="id">Entity key</param>
        /// <returns></returns>
        public ApplicationConfiguration GetById(Guid id)
        {
            return _collection.Find(Query.EQ("_id", id)).FirstOrDefault();
        }

        /// <summary>
        /// Gets a generic IQueryable member
        /// </summary>
        /// <returns></returns>
        public IQueryable<ApplicationConfiguration> AsQueryable()
        {
            return _collection.AsQueryable<ApplicationConfiguration>();
        }

        #endregion
    }
}
