//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using Takenet.Library.Logging.Filters;

//namespace Takenet.Library.Logging.LogConsumer.Factories
//{
//    public class AggregateLogListener : ILoggerFactory
//    {
//        private ICollection<ILoggerFactory> _logListeners;
//        private ILogFilter _filter;

//        #region Constructor

//        public AggregateLogListener(string applicationName, IDictionary<string, string> propertyDictionary)
//        {
//            _logListeners = new List<ILoggerFactory>();

//            if (propertyDictionary.ContainsKey("LogListeners"))
//            {
//                var logListenerTypeNames = propertyDictionary["LogListeners"].Split(';');

//                foreach (var logListenerTypeName in logListenerTypeNames)
//                {
//                    Type listenerType = Type.GetType(logListenerTypeName);

//                    if (listenerType == null)
//                    {
//                        throw new ConfigurationErrorsException(string.Format("Could not find type '{0}' for application '{1}'", logListenerTypeName, applicationName));
//                    }

//                    var listenerPropertyDictionary = propertyDictionary
//                        .Where(p => !p.Key.Contains(".") || p.Key.StartsWith(string.Format("{0}.", listenerType.Name)))
//                        .ToDictionary(e => e.Key.Replace(string.Format("{0}.", listenerType.Name), string.Empty), 
//                                      e => e.Value);

//                    ILoggerFactory listener = Activator.CreateInstance(listenerType, applicationName, listenerPropertyDictionary) as ILoggerFactory;
//                    _logListeners.Add(listener);
//                }
//            }
//            else
//            {
//                throw new ArgumentException("LogListener property is mandatory for AggregateLogListener");
//            }

//            if (propertyDictionary.ContainsKey("LogSeverity"))
//            {
//                var logSeverity = propertyDictionary["LogSeverity"];

//                TraceEventType severity;

//                if (Enum.TryParse<TraceEventType>(logSeverity, out severity))
//                {
//                    _filter = new SeverityLogFilter(severity);
//                }
//            }
//        }

//        #endregion

//        #region ILogListener Members

//        public void WriteLog(LogMessage logMessage)
//        {
//            if (_filter == null ||
//                _filter.ShouldWriteLog(logMessage))
//            {
//                foreach (var listener in _logListeners)
//                {
//                    listener.WriteLog(logMessage);
//                }
//            }
//        }

//        #endregion
//    }
//}
