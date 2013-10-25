using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Takenet.Library.Logging.Loggers
{
    /// <summary>
    /// Logs on the machine EventViewer
    /// </summary>
    public class EventViewerLogger : ILogger
    {
        #region Constructors

        public EventViewerLogger()
            : this(null)
        {

        }

        public EventViewerLogger(ILogFilter filter)
        {
            Filter = filter;
        }

        #endregion

        #region ILogger Members

        public ILogFilter Filter { get; private set; }

        public void WriteLog(LogMessage logMessage)
        {
            if (Filter == null || Filter.ShouldWriteLog(logMessage))
            {
                EventLog.WriteEntry(logMessage.ApplicationName, GetFormattedMessage(logMessage), GetEntryType(logMessage), 0, 0);
            }
        }

        #endregion

        private static EventLogEntryType GetEntryType(LogMessage logMessage)
        {

            switch (logMessage.Severity)
            {
                case TraceEventType.Critical:
                    return EventLogEntryType.Error;
                case TraceEventType.Error:
                    return EventLogEntryType.Error;
                case TraceEventType.Information:
                    return EventLogEntryType.Information;
                case TraceEventType.Verbose:
                    return EventLogEntryType.Information;
                case TraceEventType.Warning:
                    return EventLogEntryType.Warning;
                default:
                    return EventLogEntryType.Information;
            }
        }

        private static string GetFormattedMessage(LogMessage logMessage)
        {
            StringBuilder message = new StringBuilder();
            message.AppendFormat("Title: {0}", logMessage.Title);
            message.AppendLine();
            message.AppendFormat("Message: {0}", logMessage.Message);
            message.AppendLine();
            message.AppendFormat("Categories: {0}", logMessage.CategoriesFlat);
            message.AppendLine();
            message.AppendFormat("Properties: {0}", logMessage.ExtendedPropertiesFlat);

            return message.ToString();
        }
    }
}
