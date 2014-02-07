using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Takenet.Library.Data;
using Takenet.Library.Logging.Mongo;
using Takenet.Library.Logging.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Takenet.Library.Logging.Filters;

namespace Takenet.Library.Logging.LogConsumer.Factories
{
    /// <summary>
    /// Insert log messages to specific collections in MongoDB
    /// </summary>
    public class MongoDBLogFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        public ILogger Create(string applicationName, IDictionary<string, string> propertyDictionary)
        {
            string collectionName = null;

            if (propertyDictionary.ContainsKey("CollectionName"))
            {
                collectionName = propertyDictionary["CollectionName"];
            }

            int collectionSize = 0;

            if (propertyDictionary.ContainsKey("CollectionSize"))
            {
                int.TryParse(propertyDictionary["CollectionSize"], out collectionSize);
            }

            string connectionString = null;

            if (propertyDictionary.ContainsKey("ConnectionString"))
            {
                connectionString = propertyDictionary["ConnectionString"];
            }
            else if (propertyDictionary.ContainsKey("ConnectionStringName"))
            {
                connectionString = ConfigurationManager.ConnectionStrings[propertyDictionary["ConnectionStringName"]].ConnectionString;
            }
            else
            {
                connectionString = ConfigurationManager.ConnectionStrings[MongoHelper.DEFAULT_MONGODB_CONNECTIONSTRING_NAME].ConnectionString;
            }

            ILogFilter filter = null;
            if (propertyDictionary.ContainsKey("LogSeverity"))
            {
                var logSeverity = propertyDictionary["LogSeverity"];

                TraceEventType severity;

                if (Enum.TryParse<TraceEventType>(logSeverity, out severity))
                {
                    filter = new SeverityLogFilter(severity);
                }
            }

            var mongoLogger = new MongoLogger(connectionString, collectionName, collectionSize, filter);

            // Creates the application configuration on mongo if it doesn't exists
            var applicationConfigurationRepository = new Takenet.Library.Logging.Mongo.ApplicationConfigurationRepository(connectionString);

            var applicationNameRegex = new Regex(applicationName, RegexOptions.IgnoreCase);

            var applicationConfiguration = applicationConfigurationRepository
                .AsQueryable()
                .Where(a => applicationNameRegex.IsMatch(a.ApplicationName))
                .FirstOrDefault();

            if (applicationConfiguration == null)
            {
                applicationConfiguration = applicationConfigurationRepository
                                .AsQueryable()
                                .Where(a => applicationNameRegex.IsMatch(a.ApplicationName))
                                .FirstOrDefault();

                if (applicationConfiguration == null)
                {
                    applicationConfiguration = new ApplicationConfiguration()
                    {
                        ApplicationName = applicationName,
                        SeverityLevel = System.Diagnostics.TraceEventType.Warning
                    };

                    applicationConfigurationRepository.Add(applicationConfiguration, true);
                }                
            }

            return mongoLogger;
        }

        public Type LoggerType
        {
            get { return typeof(MongoLogger); }
        }

        #endregion
    }
}
