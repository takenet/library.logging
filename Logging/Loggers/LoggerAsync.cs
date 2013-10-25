using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.Library.Logging.Loggers
{
    /// <summary>
    /// Implements a simple log wrapper 
    /// for async logging. This class should
    /// be used only for loggers that don't have
    /// an explict async implementation.
    /// </summary>
    public class LoggerAsync : ILoggerAsync
    {
        private ILogger _logger;

        #region Constructor

        public LoggerAsync(ILogger logger, ILogFilter filter)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;

            this.Filter = filter;

        }

        public LoggerAsync(ILogger logger)
            : this(logger, null)
        {

        }

        #endregion

        #region ILoggerAsync Members

        public ILogFilter Filter { get; private set; }

        public Task WriteLogAsync(LogMessage logMessage)
        {
            return Task.Factory.StartNew(() =>
            {
                if (this.ShouldWriteLog(logMessage))
                {
                    _logger.WriteLog(logMessage);
                }
            });
        }

        #endregion
    }
}
