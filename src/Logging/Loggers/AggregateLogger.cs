using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Allow logging on multiple loggers
    /// </summary>
    public class AggregateLogger : ILogger
    {
        #region Constructor

        /// <summary>
        /// Initialize logger with specified loggers without filter
        /// </summary>
        /// <param name="loggers"></param>
        public AggregateLogger(params ILogger[] loggers)
            : this(null, loggers)
        {

        }

        /// <summary>
        /// Initialize logger with specified loggers and filter
        /// </summary>
        /// <param name="loggers"></param>
        public AggregateLogger(ILogFilter filter, params ILogger[] loggers)
        {
            Filter = filter;
            Loggers = loggers;
        } 

        #endregion

        /// <summary>
        /// Loggers to write log messages
        /// </summary>
        public ILogger[] Loggers { get; private set; }

        #region ILogger Members

        /// <summary>
        /// Specific a master filter for log messages
        /// </summary>
        public ILogFilter Filter { get; private set; }

        /// <summary>
        /// Write log message on configured loggers
        /// </summary>
        /// <param name="logMessage"></param>
        public void WriteLog(LogMessage logMessage)
        {
            if (this.ShouldWriteLog(logMessage))
            {
                foreach (var logger in Loggers)
                {
                    logger.WriteLog(logMessage);
                }
            }
        }

        #endregion
    }
}
