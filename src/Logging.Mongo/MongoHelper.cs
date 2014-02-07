using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Library.Data;
using MongoDB.Bson.Serialization;
using Takenet.Library.Logging.Models;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Takenet.Library.Logging.Mongo
{
    /// <summary>
    /// Helper class to work with MongoDB collections
    /// </summary>
    public static class MongoHelper
    {
        public const string DEFAULT_MONGODB_CONNECTIONSTRING_NAME = "TakenetApplicationLogConnectionString";
        public const int DEFAULT_COLLECTION_SIZE = 1073741824;


        private static Dictionary<string, MongoServer> _mongoServerDictionary = new Dictionary<string, MongoServer>();
        private static ConnectionStringSettings _connectionString;

        /// <summary>
        /// Defines type mapping for mongo driver
        /// </summary>
        static MongoHelper()
        {                        
            BsonClassMap.RegisterClassMap<ApplicationConfiguration>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);                
                cm.SetIdMember(cm.GetMemberMap(c => c.ApplicationConfigurationId));

            });

            BsonClassMap.RegisterClassMap<SeverityFilter>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.UnmapField(e => e.ApplicationConfiguration);
                cm.UnmapField(e => e.ApplicationConfigurationId);
                cm.UnmapField(e => e.SeverityLevelFlatString);
                cm.SetIdMember(cm.GetMemberMap(c => c.SeverityFilterId));
            });

            BsonClassMap.RegisterClassMap<LogMessage>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.UnmapField(e => e.CategoriesFlat);
                cm.UnmapField(e => e.ExtendedPropertiesFlat);
                cm.UnmapField(e => e.LogMessageSafeId);
                cm.SetIdMember(cm.GetMemberMap(c => c.LogMessageId));
            });

            _connectionString = ConfigurationManager.ConnectionStrings[DEFAULT_MONGODB_CONNECTIONSTRING_NAME];
        }

        /// <summary>
        /// Gets a mongodb collection from logging database
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static MongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            return GetCollection<TDocument>(collectionName, false);
        }

        /// <summary>
        /// Gets a mongodb collection from logging database
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static MongoCollection<TDocument> GetCollection<TDocument>(string collectionName, bool isCaped)
        {
            return GetCollection<TDocument>(collectionName, isCaped, 0);
        }

        /// <summary>
        /// Gets a mongodb collection from logging database
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static MongoCollection<TDocument> GetCollection<TDocument>(string collectionName, bool isCaped, int collectionSize)
        {
            return GetCollection<TDocument>(collectionName, isCaped, collectionSize, _connectionString.ConnectionString);
        }

        /// <summary>
        /// Gets a mongodb collection from logging database
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static MongoCollection<TDocument> GetCollection<TDocument>(string collectionName, bool isCaped, int collectionSize, string connectionString)
        {
            var mongoUrl = new MongoUrl(connectionString);

            var mongoServerSettings = new MongoServerSettings()
            {
                Server = mongoUrl.Server
            };

            var mongoServer = new MongoServer(mongoServerSettings);            

            var database = mongoServer.GetDatabase(mongoUrl.DatabaseName);

            if (collectionSize <= 0)
            {
                collectionSize = DEFAULT_COLLECTION_SIZE;
            }

            if (!database.CollectionExists(collectionName))
            {
                if (isCaped)
                {
                    var options = CollectionOptions.SetMaxSize(collectionSize)
                                                   .SetCapped(isCaped);

                    database.CreateCollection(collectionName, options);
                }
                else
                {
                    database.CreateCollection(collectionName);
                }
            }

            return database.GetCollection<TDocument>(collectionName);
        }
    }
}
