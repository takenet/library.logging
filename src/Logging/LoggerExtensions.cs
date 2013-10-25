using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Utility extensions for ILogger interface
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Allows to check if there's a log filter
        /// for a specific log message properties
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="severity">Level of severity of log message</param>
        /// <param name="applicationName">Name of the current application</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <returns></returns>
        public static bool ShouldWriteLog(this ILogger logger, string title, TraceEventType severity, string applicationName, params string[] categories)
        {
            return (logger.Filter == null || logger.Filter.ShouldWriteLog(new LogMessage(title, null, null, severity, applicationName, categories, 0, null)));
        }

        /// <summary>
        /// Allows to check if there's a log filter
        /// for a specific log message properties
        /// </summary>
        /// <param name="logMessage">Message to be logged</param>
        /// <returns></returns>
        public static bool ShouldWriteLog(this ILogger logger, LogMessage logMessage)
        {
            return (logger.Filter == null ||
                    (logMessage.ShouldWriteLog == null && logger.Filter.ShouldWriteLog(logMessage)) ||
                    (logMessage.ShouldWriteLog != null && logMessage.ShouldWriteLog.Value));
        }

        /// <summary>
        /// Writes a Critical log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        public static void WriteCritical(this ILogger logger, string title, string message, string userName, string applicationName, string category)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Critical, applicationName, new[] { category }, 0, null));
        }

        /// <summary>
        /// Writes a Critical log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        public static void WriteCritical(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Critical, applicationName, new[] { category }, correlationID, null));
        }

        /// <summary>
        /// Writes a Critical log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Critical to the log message</param>
        public static void WriteCritical(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID, IDictionary<string, string> extendedProperties)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Critical, applicationName, new[] { category }, correlationID, extendedProperties));
        }

        /// <summary>
        /// Writes a Critical log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="messageFunc">A function which returns the message to be logged. It will be invoked only in case that the message is not filtered.</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Verbose to the log message</param>
        public static void WriteCritical(this ILogger logger, string title, Func<string> messageFunc, string applicationName, string[] categories, string userName = null, long correlationID = 0, IDictionary<string, string> extendedProperties = null)
        {
            var logMessage = new LogMessage(title, null, userName, TraceEventType.Critical, applicationName, categories, correlationID, extendedProperties);

            if (ShouldWriteLog(logger, logMessage))
            {
                logMessage.Message = messageFunc.Invoke();
                logger.WriteLog(logMessage);
            }
        }

        /// <summary>
        /// Writes a Error log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        public static void WriteError(this ILogger logger, string title, string message, string userName, string applicationName, string category)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Error, applicationName, new[] { category }, 0, null));
        }

        /// <summary>
        /// Writes a Error log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        public static void WriteError(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Error, applicationName, new[] { category }, correlationID, null));
        }

        /// <summary>
        /// Writes a Error log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Error to the log message</param>
        public static void WriteError(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID, IDictionary<string, string> extendedProperties)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Error, applicationName, new[] { category }, correlationID, extendedProperties));
        }

        /// <summary>
        /// Writes a Error log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="messageFunc">A function which returns the message to be logged. It will be invoked only in case that the message is not filtered.</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Verbose to the log message</param>
        public static void WriteError(this ILogger logger, string title, Func<string> messageFunc, string applicationName, string[] categories, string userName = null, long correlationID = 0, IDictionary<string, string> extendedProperties = null)
        {
            var logMessage = new LogMessage(title, null, userName, TraceEventType.Error, applicationName, categories, correlationID, extendedProperties);

            if (ShouldWriteLog(logger, logMessage))
            {
                logMessage.Message = messageFunc.Invoke();
                logger.WriteLog(logMessage);
            }
        }


        /// <summary>
        /// Writes a Information log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        public static void WriteInformation(this ILogger logger, string title, string message, string userName, string applicationName, string category)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Information, applicationName, new[] { category }, 0, null));
        }

        /// <summary>
        /// Writes a Information log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        public static void WriteInformation(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Information, applicationName, new[] { category }, correlationID, null));
        }

        /// <summary>
        /// Writes a Information log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant information to the log message</param>
        public static void WriteInformation(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID, IDictionary<string, string> extendedProperties)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Information, applicationName, new[] { category }, correlationID, extendedProperties));
        }

        /// <summary>
        /// Writes a Information log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="messageFunc">A function which returns the message to be logged. It will be invoked only in case that the message is not filtered.</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Verbose to the log message</param>
        public static void WriteInformation(this ILogger logger, string title, Func<string> messageFunc, string applicationName, string[] categories, string userName = null, long correlationID = 0, IDictionary<string, string> extendedProperties = null)
        {
            var logMessage = new LogMessage(title, null, userName, TraceEventType.Information, applicationName, categories, correlationID, extendedProperties);

            if (ShouldWriteLog(logger, logMessage))
            {
                logMessage.Message = messageFunc.Invoke();
                logger.WriteLog(logMessage);
            }
        }


        /// <summary>
        /// Writes a Verbose log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        public static void WriteVerbose(this ILogger logger, string title, string message, string userName, string applicationName, string category)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Verbose, applicationName, new[] { category }, 0, null));
        }

        /// <summary>
        /// Writes a Verbose log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        public static void WriteVerbose(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Verbose, applicationName, new[] { category }, correlationID, null));
        }

        /// <summary>
        /// Writes a Verbose log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Verbose to the log message</param>
        public static void WriteVerbose(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID, IDictionary<string, string> extendedProperties)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Verbose, applicationName, new[] { category }, correlationID, extendedProperties));
        }

        /// <summary>
        /// Writes a Verbose log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="messageFunc">A function which returns the message to be logged. It will be invoked only in case that the message is not filtered.</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Verbose to the log message</param>
        public static void WriteVerbose(this ILogger logger, string title, Func<string> messageFunc, string applicationName, string[] categories, string userName = null, long correlationID = 0, IDictionary<string, string> extendedProperties = null)
        {
            var logMessage = new LogMessage(title, null, userName, TraceEventType.Verbose, applicationName, categories, correlationID, extendedProperties);

            if (ShouldWriteLog(logger, logMessage))
            {
                logMessage.Message = messageFunc.Invoke();
                logger.WriteLog(logMessage);
            }
        }

        /// <summary>
        /// Writes a Warning log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        public static void WriteWarning(this ILogger logger, string title, string message, string userName, string applicationName, string category)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Warning, applicationName, new[] { category }, 0, null));
        }

        /// <summary>
        /// Writes a Warning log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        public static void WriteWarning(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Warning, applicationName, new[] { category }, correlationID, null));
        }

        /// <summary>
        /// Writes a Warning log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="category">Category where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Warning to the log message</param>
        public static void WriteWarning(this ILogger logger, string title, string message, string userName, string applicationName, string category, long correlationID, IDictionary<string, string> extendedProperties)
        {
            logger.WriteLog(new LogMessage(title, message, userName, TraceEventType.Warning, applicationName, new[] { category }, correlationID, extendedProperties));
        }

        /// <summary>
        /// Writes a Warning log message with current logger instance
        /// </summary>
        /// <param name="logger">Current logger instance</param>
        /// <param name="title">Title to the log message</param>
        /// <param name="messageFunc">A function which returns the message to be logged. It will be invoked only in case that the message is not filtered.</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant Verbose to the log message</param>
        public static void WriteWarning(this ILogger logger, string title, Func<string> messageFunc, string applicationName, string[] categories, string userName = null, long correlationID = 0, IDictionary<string, string> extendedProperties = null)
        {
            var logMessage = new LogMessage(title, null, userName, TraceEventType.Warning, applicationName, categories, correlationID, extendedProperties);

            if (ShouldWriteLog(logger, logMessage))
            {
                logMessage.Message = messageFunc.Invoke();
                logger.WriteLog(logMessage);
            }
        }
    }
}
