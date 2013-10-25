using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Represents a application log message
    /// </summary>
    [Serializable]
    public class LogMessage
    {
        #region Constructor

        /// <summary>
        /// Create a instance with default values
        /// </summary>
        public LogMessage()
            : this(null, null, Environment.UserName, TraceEventType.Verbose, "Unknown", new string[0], 0, new Dictionary<string, string>())
        {
        }

        /// <summary>
        /// Create a instance with specified values
        /// </summary>
        /// <param name="title">Title to the log message</param>
        /// <param name="message">The actual log message</param>
        /// <param name="userName">Name of user who is interacting with the system</param>
        /// <param name="severity">Level of severity of log message</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <param name="correlationID">Identifier to correlate this log to other log entries</param>
        /// <param name="extendedProperties">Pairs of name-value containing relevant information to the log message</param>
        public LogMessage(string title, string message, string userName, TraceEventType severity, string applicationName, string[] categories, long correlationID, IDictionary<string, string> extendedProperties)
        {
            LogMessageId = Guid.NewGuid();

            Title = title;
            Message = message;
            UserName = userName;
            Severity = severity;
            Timestamp = DateTime.UtcNow;

            // Application name is required
            if (!string.IsNullOrEmpty(applicationName))
            {
                ApplicationName = applicationName;
            }
            else
            {
                throw new ArgumentNullException("applicationName");
            }

            var currentProcess = Process.GetCurrentProcess();

            ProcessName = currentProcess.ProcessName;
            MachineName = Environment.MachineName;
            ProcessId = currentProcess.Id;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            Categories = categories;
            CorrelationId = correlationID;
            ExtendedProperties = extendedProperties;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Unique identifier of the log message
        /// </summary>
        public Guid LogMessageId { get; set; }

        /// <summary>
        /// Database-safe log message identifier
        /// </summary>
        public long LogMessageSafeId { get; set; }

        /// <summary>
        /// Date and time of the log event, UTC time
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Title to the log message
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The actual log message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Name of user who is interacting with the system
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Level of severity of log message
        /// </summary>
        public TraceEventType Severity { get; set; }

        public int SeverityFlat
        {
            get { return (int)Severity; }
            set { Severity = (TraceEventType)value; }
        }

        /// <summary>
        /// Name of current application
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Name of executing process
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Name of the machine where the process is running
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Identifier of process on OS
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Identifier of current thread
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Categories where the current log message fits
        /// </summary>
        public string[] Categories { get; set; }

        private string _categoriesFlat;
        public string CategoriesFlat
        {
            get 
            {
                if (_categoriesFlat == null)
                {
                    _categoriesFlat = Categories != null && Categories.Length > 0 ? Categories.Aggregate((a, b) => a + ";" + b) : null; 
                }

                return _categoriesFlat; 
            }
            set 
            { 
                Categories = value != null ? value.Split(';') : null;
                _categoriesFlat = value;
            }
        }

        /// <summary>
        /// Identifier to correlate this log to other log entries
        /// </summary>
        public long CorrelationId { get; set; }

        /// <summary>
        /// Pairs of name-value containing relevant information to the log message
        /// </summary>
        public IDictionary<string, string> ExtendedProperties { get; set; }

        private string _extendedPropertiesFlat;
        public string ExtendedPropertiesFlat
        {
            get
            {
                if (_extendedPropertiesFlat == null)
                {
                    if (ExtendedProperties != null &&
                        ExtendedProperties.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var pair in ExtendedProperties)
                        {
                            sb.AppendFormat("{0}={1};", pair.Key, pair.Value);
                        }

                        _extendedPropertiesFlat = sb.ToString(0, sb.Length - 1);
                    }
                    else
                    {
                        _extendedPropertiesFlat = null;
                    }
                }

                return _extendedPropertiesFlat;
            }
            set
            {
                if (value != null)
                {
                    var dic = new Dictionary<string, string>();
                    var properties = value.Split(';');
                    foreach (var property in properties)
                    {
                        var prop = property.Split('=');
                        if (dic.ContainsKey(prop.First()))
                        {
                            continue;
                        }
                        dic.Add(prop.First(),prop.Last());
                    }
                    ExtendedProperties = dic;
                }
                else
                {
                    ExtendedProperties = null;
                }

                _extendedPropertiesFlat = value;
            }
        }

        #endregion

        [NonSerialized]
        private bool? _shouldWriteLog;

        /// <summary>
        /// Allow to the log filters set a flag
        /// to avoid double filtering check
        /// </summary>
        internal bool? ShouldWriteLog
        {
            get { return _shouldWriteLog; }
            set { _shouldWriteLog = value; }
        }

        /// <summary>
        /// Get a string from the log message instance
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Timestamp: {0} - Severity: {1} - Title: {2} - Message: {3}", Timestamp, Severity, Title, Message);
        }
    }
}
