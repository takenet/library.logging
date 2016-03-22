using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Takenet.Library.Logging.LogConsumer.Configuration;
using System.Configuration;
using Takenet.Library.Logging.Loggers;
using Takenet.Library.Logging.Filters;
using System.Reflection;

namespace Takenet.Library.Logging.LogConsumer
{
    /// <summary>
    /// Consume log from application queues and send it to the listeners
    /// </summary>
    public partial class LogConsumerService : ServiceBase
    {
        private List<ApplicationLogConsumer> _applicationLogConsumerCollection;
        private ILogger _logger;

        private IDictionary<Type, ILoggerFactory> _loggerFactoryDictionary;

        /// <summary>
        /// Service constructor
        /// </summary>
        public LogConsumerService()
        {
            InitializeComponent();
            
            TraceEventType logSeverity;
            if (!Enum.TryParse(ConfigurationManager.AppSettings["LogSeverity"], out logSeverity))
            {
                logSeverity = TraceEventType.Verbose;
            }

#if DEBUG
            _logger = new DebugLogger(new SeverityLogFilter(logSeverity));
#else
            _logger = new EventViewerLogger(new SeverityLogFilter(logSeverity)); 
#endif

            _loggerFactoryDictionary = new Dictionary<Type, ILoggerFactory>();

            var loggerFactoryTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ILoggerFactory).IsAssignableFrom(t) && t.IsClass == true);

            foreach (var type in loggerFactoryTypes)
            {
                var factory = (ILoggerFactory)Activator.CreateInstance(type);

                if (!_loggerFactoryDictionary.ContainsKey(type))
                {
                    _loggerFactoryDictionary.Add(type, factory);
                }
                else
                {
                    _logger.WriteCritical(
                        "LogConsumerService", 
                        string.Format("There's more then a factory defined for logger '{0}'", type),
                        null,
                        "LogConsumer",
                        "Consumer"
                        );
                }
            }

        }
        
        #region ServiceBase Members

        /// <summary>
        /// Executed when the service is started by Windows
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            StartConsumers();
        }

        /// <summary>
        /// Executed when a service stop is requested by Windows
        /// </summary>
        protected override void OnStop()
        {
            StopConsumers();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Start the queue consumers
        /// </summary>
        public void StartConsumers()
        {
            try
            {
                _logger.WriteInformation(
                    "StartConsumers",
                    "Starting LogConsumerService...",
                    null,
                    "LogConsumer",
                    "Service"
                    );

                _applicationLogConsumerCollection = new List<ApplicationLogConsumer>();

                foreach (LogConsumerApplicationElement application in LogConsumerConfiguration.Section.ApplicationCollection)
                {
                    string applicationName = application.Name;
                    string queuePath = application.QueuePath;

                    if (string.IsNullOrEmpty(queuePath))
                    {
                        queuePath = QueueLogger.GetQueuePath(applicationName);
                    }

                    int consumersCount = application.ConsumersCount;

                    var propertyDictionary = new Dictionary<string, string>();

                    if (application.PropertyCollection != null)
                    {
                        foreach (LogConsumerApplicationPropertyElement property in application.PropertyCollection)
                        {
                            propertyDictionary.Add(property.Name, property.Value);
                        }
                    }

                    Type loggerType = Type.GetType(application.LoggerType);

                    if (loggerType == null ||
                        !typeof(ILogger).IsAssignableFrom(loggerType))
                    {
                        throw new ConfigurationErrorsException(string.Format("Invalid logger type '{0}' for application '{1}'", application.LoggerType, applicationName));
                    }

                    ILoggerFactory factory;

                    if (!_loggerFactoryDictionary.TryGetValue(loggerType, out factory))
                    {
                        throw new ConfigurationErrorsException(string.Format("There's no factory defined for logger type '{0}'", application.LoggerType));
                    }

                    var logger = factory.Create(application.Name, propertyDictionary);

                    var applicationLogConsumer = new ApplicationLogConsumer(logger, applicationName, queuePath, consumersCount, _logger);

                    _applicationLogConsumerCollection.Add(applicationLogConsumer);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteCritical(
                    "StartConsumers",
                    ex.ToString(),
                    null,
                    "LogConsumer",
                    "Exception"
                    );

                throw;
            }
        }

        /// <summary>
        /// Stop the queue consumers
        /// </summary>
        public void StopConsumers()
        {
            _logger.WriteInformation(
                "StopConsumers",
                "Stopping LogConsumerService...",
                null,
                "LogConsumer",
                "Service"
                );

            foreach (var applicationLogConsumer in _applicationLogConsumerCollection)
            {
                applicationLogConsumer.Dispose();
            }
        }

        #endregion

    }
}
