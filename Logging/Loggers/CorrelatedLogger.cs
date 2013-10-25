using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.Library.Logging.Loggers
{
    /// <summary>
    /// Caches filtered log messages and write then
    /// if a correlated message is not filtered
    /// </summary>
    internal class CorrelatedLogger : ILogger, IDisposable
    {
        private ILogger _logger;
        private ConcurrentDictionary<long, ConcurrentBag<LogMessage>> _filteredMessageBuffer;

        private const int MAX_BUFFER_SIZE = 1000;

        #region Constructor

        public CorrelatedLogger(ILogFilter filter, ILogger logger)
        {           
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            _logger = logger;
            Filter = filter;

            _filteredMessageBuffer = new ConcurrentDictionary<long, ConcurrentBag<LogMessage>>();
        }

        #endregion

        #region ILogger Members

        public ILogFilter Filter { get; private set; }

        public void WriteLog(LogMessage logMessage)
        {
            if (Filter.ShouldWriteLog(logMessage))
            {
                _logger.WriteLog(logMessage);

                // Check if theres any buffered related messages to be written
                if (_filteredMessageBuffer.ContainsKey(logMessage.CorrelationId))
                {
                    var filteredLogMessageList = _filteredMessageBuffer[logMessage.CorrelationId];

                    foreach (var filteredLogMessage in filteredLogMessageList)
                    {
                        _logger.WriteLog(filteredLogMessage);
                    }

                    while (!filteredLogMessageList.IsEmpty)
                    {
                        LogMessage filteredLogMessage = null;
                        filteredLogMessageList.TryTake(out filteredLogMessage);
                    }
                }
            }
            else if (_filteredMessageBuffer.Count < MAX_BUFFER_SIZE)
            {                
                if (!_filteredMessageBuffer.ContainsKey(logMessage.CorrelationId))
                {
                    var logMessageBuffer = new ConcurrentBag<LogMessage>();
                    _filteredMessageBuffer.TryAdd(logMessage.CorrelationId, logMessageBuffer);
                }

                // Adds the filtered log message to the buffer
                _filteredMessageBuffer[logMessage.CorrelationId].Add(logMessage);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _filteredMessageBuffer.Clear();
        }

        #endregion
    }
}
