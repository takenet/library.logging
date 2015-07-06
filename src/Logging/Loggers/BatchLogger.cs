using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace Takenet.Library.Logging.Loggers
{
    /// <summary>
    /// Provides a batch logging service.
    /// </summary>
    public sealed class BatchLogger : ILoggerAsync, ILogger, IDisposable
    {
        private readonly Func<LogMessage, Task> _writeLogFunc;
        private readonly ConcurrentQueue<LogMessage> _messageQueue;
        private readonly int _batchSize;
        private readonly SemaphoreSlim _writeSemaphoreSlim;
        private readonly System.Timers.Timer _flushTimer;

        public BatchLogger(ILoggerAsync logger, ILogFilter filter = null, int batchSize = 10, TimeSpan flushInterval = default(TimeSpan))
            : this(filter, batchSize, flushInterval)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (logger is BatchLogger) throw new ArgumentException("The logger cannot be a BatchLogger instance", "logger");
            Filter = filter;
            _writeLogFunc = logger.WriteLogAsync;
        }

        public BatchLogger(ILogger logger, ILogFilter filter = null, int batchSize = 10, TimeSpan flushInterval = default(TimeSpan))
            : this(filter, batchSize, flushInterval)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (logger is BatchLogger) throw new ArgumentException("The logger cannot be a BatchLogger instance", "logger");

            Filter = filter;
            _writeLogFunc = m =>
            {
                logger.WriteLog(m);
                return Task.FromResult(0);
            };
        }

        private BatchLogger(ILogFilter filter, int batchSize, TimeSpan flushInterval)
        {
            Filter = filter;

            if (batchSize <= 0) throw new ArgumentException("The batch size value must be positive", "batchSize");
            _batchSize = batchSize;
            _messageQueue = new ConcurrentQueue<LogMessage>();
            _writeSemaphoreSlim = new SemaphoreSlim(1);

            if (flushInterval.Equals(default(TimeSpan)))
            {
                flushInterval = TimeSpan.FromSeconds(60);
            }
            _flushTimer = new System.Timers.Timer(flushInterval.TotalMilliseconds);
            _flushTimer.Elapsed += async (sender, args) =>
            {
                await _writeSemaphoreSlim.WaitAsync().ConfigureAwait(false);
                try
                {
                    await FlushAsync();
                }
                catch (Exception ex)
                {
                    var flushException = FlushException;
                    if (flushException != null)
                    {
                        flushException(this, new ExceptionEventArgs(ex));
                    }
                }
                finally
                {
                    _writeSemaphoreSlim.Release();
                }

            };
            _flushTimer.Start();
            
        }

        #region ILogger Members

        public ILogFilter Filter { get; private set; }

        public void WriteLog(LogMessage logMessage)
        {
            WriteLogAsync(logMessage).Wait();
        }        

        #endregion

        public event EventHandler<ExceptionEventArgs> FlushException;

        #region ILoggerAsync Members


        public async Task WriteLogAsync(LogMessage logMessage)
        {
            if (LoggerAsyncExtensions.ShouldWriteLog(this, logMessage))
            {
                _messageQueue.Enqueue(logMessage);
                if (_messageQueue.Count >= _batchSize)
                {
                    await _writeSemaphoreSlim.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        if (_messageQueue.Count >= _batchSize)
                        {
                            await FlushAsync().ConfigureAwait(false);
                        }
                    }
                    finally
                    {
                        _writeSemaphoreSlim.Release();
                    }
                }
            }

        }

        private async Task FlushAsync()
        {
            while (!_messageQueue.IsEmpty)
            {                
                LogMessage queueLogMessage;
                if (!_messageQueue.TryDequeue(out queueLogMessage))
                {
                    break;
                }

                await _writeLogFunc(queueLogMessage).ConfigureAwait(false);
            }
        }

        #endregion

        public void Dispose()
        {
            _flushTimer.Dispose();
        }
    }

    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
