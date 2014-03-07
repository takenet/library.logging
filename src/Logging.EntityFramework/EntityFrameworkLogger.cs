using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Library.Data;
using Takenet.Library.Logging;
using Takenet.Library.Logging.EntityFramework.Repositories;

namespace Takenet.Library.Logging.EntityFramework
{
    public class EntityFrameworkLogger : ILogger, ILoggerAsync
    {
        private string _nameOrConnectionString;

        #region Constructor

        public EntityFrameworkLogger()
            : this(null)
        {
        }

        public EntityFrameworkLogger(ILogFilter filter)
            : this(null, filter)
        {

        }

        public EntityFrameworkLogger(string nameOrConnectionString, ILogFilter filter)
        {
            _nameOrConnectionString = nameOrConnectionString;
            Filter = filter;
        }

        #endregion

        #region ILogger Members

        public ILogFilter Filter { get; private set; }

        public void WriteLog(LogMessage logMessage)
        {
            if (((ILogger)this).ShouldWriteLog(logMessage))
            {
                using (var unitOfWork = CreateContext())
                {
                    var logMessageRepository = new LogMessageRepository(unitOfWork);
                    logMessageRepository.AddAsync(logMessage, true).Wait();
                    unitOfWork.SaveAsync().Wait();
                }
            }
        }

        #endregion

        #region ILoggerAsync Members


        public System.Threading.Tasks.Task WriteLogAsync(LogMessage logMessage)
        {
            throw new NotImplementedException();
        }

        #endregion

        private IUnitOfWorkAsync CreateContext()
        {
            LoggingContext context;

            if (string.IsNullOrWhiteSpace(_nameOrConnectionString))
            {
                context = new LoggingContext();
            }
            else
            {
                context = new LoggingContext(_nameOrConnectionString);
            }

            // Disable EF options to improve performance
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;

            return context;
        }


    }
}
