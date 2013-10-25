using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Takenet.Library.Logging.Loggers
{
    /// <summary>
    /// Write log on text files
    /// </summary>
    public class TextFileLogger : ILogger
    {
        private string _logFileName;

        #region Constructor

        /// <summary>
        /// Instantiate a new logger with default parameters
        /// </summary>
        public TextFileLogger()
            : this(null)
        {
        }

        /// <summary>
        /// Instantiate a new logger using
        /// specified filter
        /// </summary>
        public TextFileLogger(ILogFilter filter)
            : this(filter, null)
        {
        }

        /// <summary>
        /// Instantiate a new logger using
        /// specified filter and file name
        /// </summary>
        public TextFileLogger(ILogFilter filter, string logFileName)
        {
            Filter = filter;
            _logFileName = logFileName;

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
                string logFileName = _logFileName;

                if (string.IsNullOrWhiteSpace(logFileName))
                {
                    logFileName = string.Format("{0}_{1:yyyyMMdd}.log", logMessage.ApplicationName, DateTime.UtcNow);
                }

                if (!Path.IsPathRooted(logFileName))
                {
                    logFileName = Path.Combine(Environment.CurrentDirectory, logFileName);
                }

                if (this.ShouldWriteLog(logMessage))
                {
                    using (FileStream fileStream = new FileStream(logFileName, FileMode.Append, FileAccess.Write))
                    {
                        StreamWriter writer = new StreamWriter(fileStream);

                        string extendedProperties = null;

                        if (logMessage.ExtendedProperties != null)
                        {
                            extendedProperties = string.Join(";", logMessage.ExtendedProperties.Select(e => string.Format("{0}={1}", e.Key, e.Value)));
                        }

                        writer.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", logMessage.Timestamp, logMessage.Severity, logMessage.Title, logMessage.Message, logMessage.ApplicationName, logMessage.UserName, extendedProperties);
                        writer.Close();
                    }
                }
            }
        }

        #endregion
    }
}
