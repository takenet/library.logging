using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Takenet.Library.Logging.Filters;
using Takenet.Library.Logging.Loggers;

namespace Takenet.Library.Logging.LogConsumer.Listeners
{
    public class TextFileLogListener : ILogListener
    {
        private ILogger _logger;

        #region Constructor

        public TextFileLogListener(string applicationName, IDictionary<string, string> propertyDictionary)
        {

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

            _logger = new TextFileLogger(filter);
        }

        #endregion

        #region ILogListener Members

        /// <summary>
        /// Write the message to application collection
        /// </summary>
        /// <param name="logMessage"></param>
        public void WriteLog(LogMessage logMessage)
        {
            _logger.WriteLog(logMessage);
        }

        #endregion
    }
}
