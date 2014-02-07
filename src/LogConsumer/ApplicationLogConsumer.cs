using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Takenet.Library.Logging.LogConsumer
{
    /// <summary>
    /// Consumes logs from a application queue
    /// </summary>
    public class ApplicationLogConsumer : IDisposable
    {
        #region Private fields

        private Thread[] _threadArray;
        private ILogger _logger;
        private string _applicationName;
        private string _queuePath;
        private bool _disposed;
        private ILogger _alnternateLogger;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a instance of ApplicationLogConsumer
        /// </summary>
        /// <param name="logListener"></param>
        /// <param name="applicationName"></param>
        /// <param name="queuePath"></param>
        /// <param name="consumersCount"></param>
        public ApplicationLogConsumer(ILogger logger, string applicationName, string queuePath, int consumersCount, ILogger alternateLogger)
        {
            _logger = logger;
            _applicationName = applicationName;
            _queuePath = queuePath;
            _alnternateLogger = alternateLogger;

            _threadArray = new Thread[consumersCount];

            for (int i = 0; i < consumersCount; i++)
            {
                _threadArray[i] = new Thread(new ThreadStart(ConsumeLogs));
                _threadArray[i].IsBackground = true;
                _threadArray[i].Start();
            }
        }

        #endregion

        #region Private methods

        private void ConsumeLogs()
        {
            while (!_disposed)
            {
                try
                {
                    var logMessage = QueueLogger.DequeueLog(_applicationName, _queuePath);
                    _logger.WriteLog(logMessage);
                }
                catch (Exception ex)
                {
                    _alnternateLogger.WriteCritical(
                        "ConsumeLogs",
                        ex.ToString(),
                        null,
                        "LogConsumer",
                        "Exception"
                        );

                    throw;
                }
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Abort all consumer threads
        /// </summary>
        public void Dispose()
        {
            _disposed = true;

            foreach (var thread in _threadArray)
            {
                try
                {
                    thread.Abort();
                }
                catch { }
            }
        }

        #endregion
    }
}
