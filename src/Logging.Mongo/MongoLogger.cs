using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Library.Data;
using System.Configuration;

namespace Takenet.Library.Logging.Mongo
{
    [Serializable]
    public class MongoLogger : ILogger
    {
        private string _connectionString;
        private string _collectionName;
        private int _collectionSize;

        #region Constructors

        /// <summary>
        /// Instantiate a new MongoLogger class
        /// without specifying a log filter
        /// </summary>
        /// <param name="connectionStringName">MongoDB connection string</param>
        public MongoLogger(string nameOrConnectionString)
            : this(nameOrConnectionString, null)
        {

        }

        /// <summary>
        /// Instantiate a new MongoLogger class
        /// without specifying a log filter
        /// </summary>
        public MongoLogger(string nameOrConnectionString, string collectionName, int collectionSize)
            : this(nameOrConnectionString, collectionName, 0, null)
        {
        }

        /// <summary>
        /// Instantiate a new MongoLogger class
        /// using specified log filter
        /// </summary>
        /// <param name="filter"></param>
        public MongoLogger(string nameOrConnectionString, ILogFilter filter)
            : this(nameOrConnectionString, null, 0, filter)
        {
        }

        /// <summary>
        /// Instantiate a new MongoLogger class
        /// using specified log filter
        /// </summary>
        /// <param name="filter"></param>
        public MongoLogger(string nameOrConnectionString, string collectionName, int collectionSize, ILogFilter filter)
        {
            if (nameOrConnectionString.StartsWith("name="))
            {
                string connectionStringName = nameOrConnectionString.Replace("name=", string.Empty);

                var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];

                if (connectionString == null)
                {
                    throw new ArgumentException("Invalid connection string name");
                }

                _connectionString = connectionString.ConnectionString;
            }
            else
            {
                _connectionString = nameOrConnectionString;
            }
            
            _collectionName = collectionName;
            _collectionSize = collectionSize;
            Filter = filter;
        }

        #endregion

        #region ILogger Members

        /// <summary>
        /// Current filter of logger
        /// </summary>
        public ILogFilter Filter { get; set; }

        /// <summary>
        /// Write a log message directly 
        /// to the mongo db instance
        /// </summary>
        /// <param name="logMessage"></param>
        public void WriteLog(LogMessage logMessage)
        {
            if (this.ShouldWriteLog(logMessage))
            {
                string collectionName = _collectionName;

                if (string.IsNullOrEmpty(collectionName))
                {
                    collectionName = string.Format("{0}Logs", logMessage.ApplicationName);
                }

                IEntityRepository<LogMessage, Guid> logMessageRepository = new LogMessageRepository(collectionName, true, _collectionSize, _connectionString);
                logMessageRepository.Add(logMessage, true);
            }
        }

        #endregion
    }
}
