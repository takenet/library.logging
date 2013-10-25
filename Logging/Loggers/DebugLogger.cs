using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Write log messages on debug trace listener
    /// </summary>
    public class DebugLogger : ILogger
    {        
        #region Constructor
        /// <summary>
        /// Instantiate a new DebugLogger class
        /// without specifying a log filter
        /// </summary>
        public DebugLogger()
        {            
        }

        /// <summary>
        /// Instantiate a new QueueLogger class
        /// using specified log filter
        /// </summary>
        /// <param name="filter"></param>
        public DebugLogger(ILogFilter filter)
        {
            Filter = filter;
        } 
        #endregion

        #region ILogger Members

        /// <summary>
        /// Current filter of logger
        /// </summary>
        public ILogFilter Filter { get; set; }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="logMessage"></param>
        public void WriteLog(LogMessage logMessage)
        {
            if (this.ShouldWriteLog(logMessage))
            {                
                using (DefaultTraceListener traceListener = new DefaultTraceListener())
                {
                    traceListener.WriteLine(string.Format("Timestamp: {0}", logMessage.Timestamp));
                    traceListener.WriteLine(string.Format("Severity: {0}", logMessage.Severity));
                    traceListener.WriteLine(string.Format("ApplicationName: {0}", new object[] { logMessage.ApplicationName }));
                    traceListener.WriteLine(string.Format("Title: {0}", new object[] { logMessage.Title }));
                    traceListener.WriteLine(string.Format("UserName: {0}", new object[] { logMessage.UserName }));
                    traceListener.WriteLine(string.Format("Message: {0}", new object[] { logMessage.Message }));
                    traceListener.WriteLine(string.Format("CorrelationId: {0}", logMessage.CorrelationId));

                    string extendedProperties = null;
                    if (logMessage.ExtendedProperties != null)
                    {
                        extendedProperties = string.Join(";", logMessage.ExtendedProperties.Select(e => string.Format("{0}={1}", e.Key, e.Value)));
                    }
                    traceListener.WriteLine(string.Format("ExtendedProperties: {0}", new object[] { extendedProperties }));

                    string categories = null;
                    if (logMessage.Categories != null)
                    {
                        categories = string.Join(", ", logMessage.Categories);
                    }
                    traceListener.WriteLine(string.Format("Categories: {0}", new object[] { categories }));
                    traceListener.WriteLine(string.Format(string.Empty));

                    traceListener.Close();
                }
            }
        }

        #endregion
    }
}
