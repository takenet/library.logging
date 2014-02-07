using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Takenet.Library.Data;
using Takenet.Library.Logging.EntityFramework;
using Takenet.Library.Logging.EntityFramework.Repositories;
using Takenet.Library.Logging.Filters;

namespace Takenet.Library.Logging.LogConsumer.Factories
{
    public class EntityFrameworkLogFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        public ILogger Create(string applicationName, IDictionary<string, string> propertyDictionary)
        {
            string nameOrConnectionString = null;

            if (propertyDictionary.ContainsKey("ConnectionString"))
            {
                nameOrConnectionString = propertyDictionary["ConnectionString"];
            }
            else if (propertyDictionary.ContainsKey("ConnectionStringName"))
            {
                nameOrConnectionString = string.Format("name={0}", propertyDictionary["ConnectionStringName"]);
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

            ILogger logger;

            if (!string.IsNullOrWhiteSpace(nameOrConnectionString))
            {
                logger = new EntityFrameworkLogger(nameOrConnectionString, filter);
            }
            else
            {
                logger = new EntityFrameworkLogger(filter);
            }

            // Checks if the application configuration exists 
            // and if don't, creates it
            using (var context = GetContext(nameOrConnectionString))
            {
                var applicationConfigurationRepository = new ApplicationConfigurationRepository(context);
                var applicationConfiguration = applicationConfigurationRepository
                    .AsQueryable()
                    .Where(a => a.ApplicationName == applicationName)
                    .FirstOrDefault();

                if (applicationConfiguration == null)
                {
                    applicationConfiguration = new Models.ApplicationConfiguration();
                    applicationConfiguration.ApplicationName = applicationName;
                    applicationConfigurationRepository.Add(applicationConfiguration, true);
                    context.Save();
                }
            }

            return logger;
        }

        public Type LoggerType
        {
            get { return typeof(EntityFrameworkLogger); }
        }

        #endregion

        #region Private methods

        private IUnitOfWork GetContext(string nameOrConnectionString)
        {
            if (!string.IsNullOrWhiteSpace(nameOrConnectionString))
            {
                return new LoggingContext(nameOrConnectionString);
            }
            else
            {
                return new LoggingContext();
            }
        }

        #endregion
    }
}
