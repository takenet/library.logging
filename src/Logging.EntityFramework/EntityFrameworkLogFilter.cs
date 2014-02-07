using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Library.Logging.EntityFramework.Repositories;
using Takenet.Library.Logging.Models;

namespace Takenet.Library.Logging.EntityFramework
{
    public class EntityFrameworkLogFilter : ILogFilter
    {
        private string _nameOrConnectionString;

        public const int SEVERITY_FILTER_LIMIT = 500;

        #region Constructor

        public EntityFrameworkLogFilter()
        {

        }

        public EntityFrameworkLogFilter(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        #endregion

        #region ILogFilter Members

        public bool ShouldWriteLog(LogMessage logMessage)
        {
            bool shouldLog = false;

            using (var context = CreateContext())
            {
                var applicationConfigurationRepository = new ApplicationConfigurationRepository(context);


                var application = applicationConfigurationRepository
                                                            .AsQueryable()
                                                            .Where(a => a.ApplicationName == logMessage.ApplicationName)
                                                            .FirstOrDefault();

                // Creates the app configuration if not exists
                if (application == null)
                {
                    application = new ApplicationConfiguration()
                    {
                        ApplicationName = logMessage.ApplicationName
                    };

                    applicationConfigurationRepository.Add(application, true);
                    context.Save();
                }

                bool applicationHasChanged = false;


                foreach (var categoryName in logMessage.Categories)
                {
                    if (!application.SeverityFilterCollection.Any(s =>
                            s.CategoryName == categoryName &&
                            s.MachineName == logMessage.MachineName &&
                            s.MessageTitle == logMessage.Title) &&
                        application.SeverityFilterCollection.Count < SEVERITY_FILTER_LIMIT)
                    {
                        application.SeverityFilterCollection.Add(new SeverityFilter()
                        {
                            ApplicationConfigurationId = application.ApplicationConfigurationId,
                            CategoryName = categoryName,
                            MachineName = logMessage.MachineName,
                            MessageTitle = logMessage.Title
                        });

                        applicationConfigurationRepository.Add(application, false);
                        context.Save();
                    }
                }

                // Checks the log criterias
                if (logMessage.Severity <= application.SeverityLevel &&             // Global Severity
                    !application.SeverityFilterCollection.Any(s =>                  // Specific severity
                        logMessage.Categories.Any(cn => cn == s.CategoryName) &&
                        s.MachineName == logMessage.MachineName &&
                        s.MessageTitle == logMessage.Title &&
                        s.SeverityLevel > logMessage.Severity))
                {
                    shouldLog = true;
                }
            }

            return shouldLog;
        }


        #endregion

        private LoggingContext CreateContext()
        {
            if (string.IsNullOrWhiteSpace(_nameOrConnectionString))
            {
                return new LoggingContext();
            }
            else
            {
                return new LoggingContext(_nameOrConnectionString);
            }
        }
    }
}
