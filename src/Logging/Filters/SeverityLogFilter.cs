using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Takenet.Library.Logging.Filters
{
    /// <summary>
    /// Filter the log messages using
    /// the severity property of log messages
    /// </summary>
    [Serializable]
    public class SeverityLogFilter : ILogFilter
    {
        /// <summary>
        /// Start a new log filter that filters by Warning severity
        /// </summary>
        public SeverityLogFilter()
            : this(TraceEventType.Warning)
        {
            
        }

        /// <summary>
        /// Start a new log filter by specified severity
        /// </summary>
        /// <param name="logSeverity"></param>
        public SeverityLogFilter(TraceEventType logSeverity)
        {
            LogSeverity = logSeverity;
        }

        /// <summary>
        /// Max severity which log messages are filtered
        /// </summary>
        public TraceEventType LogSeverity { get; private set; }

        #region ILogFilter Members

        /// <summary>
        /// Indicates if the specified message should be logged
        /// </summary>
        /// <param name="logMessage"></param>
        /// <returns></returns>
        public bool ShouldWriteLog(LogMessage logMessage)
        {
            return logMessage.Severity <= LogSeverity;
        }

        #endregion
    }
}
