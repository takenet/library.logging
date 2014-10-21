using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.Library.Logging.Loggers
{
    /// <summary>
    /// Write the messages using the 
    /// Trace API from .NET.
    /// </summary>
    public class TraceLogger : ILogger
    {
        private const string MESSAGE_FORMAT = "Title: {0} - Message: {1} - User: {2} - Correlation: {3} - Categories: {4} - Properties: {5}";

        public TraceLogger()
            : this(null)
        {

        }

        public TraceLogger(ILogFilter filter)
        {
            Filter = filter;
        }

        #region ILogger Members

        public ILogFilter Filter { get; private set; }

        public void WriteLog(LogMessage logMessage)
        {
            if (this.ShouldWriteLog(logMessage))
            {
                switch (logMessage.Severity)
                {
                    case TraceEventType.Critical:
                    case TraceEventType.Error:
                        Trace.TraceError(MESSAGE_FORMAT, GetMessageFormat(logMessage));
                        break;
                    case TraceEventType.Warning:
                        Trace.TraceWarning(MESSAGE_FORMAT, GetMessageFormat(logMessage));
                        break;
                    case TraceEventType.Information:
                    case TraceEventType.Verbose:                        
                    default:
                        Trace.TraceInformation(MESSAGE_FORMAT, GetMessageFormat(logMessage));
                        break;
                }
            }
        }

        #endregion

        private static object[] GetMessageFormat(LogMessage logMessage)
        {
            return new object[] 
            { 
                logMessage.Title, 
                logMessage.Message, 
                logMessage.UserName, 
                logMessage.CorrelationId, 
                logMessage.CategoriesFlat,
                logMessage.ExtendedPropertiesFlat 
            };
        }
    }
}
