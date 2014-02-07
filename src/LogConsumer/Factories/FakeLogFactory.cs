using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.Library.Logging.LogConsumer.Factories
{
    /// <summary>
    /// Fake listener to automaticaly purge log queues
    /// </summary>
    public class FakeLogFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        public ILogger Create(string applicationName, IDictionary<string, string> propertyDictionary)
        {
            return new FakeLogger();
        }

        public Type LoggerType
        {
            get { return typeof(FakeLogger); }
        }

        #endregion

        private class FakeLogger : ILogger
        {
            #region ILogger Members

            public ILogFilter Filter
            {
                get { return null; }
            }

            public void WriteLog(LogMessage logMessage)
            {

            }

            #endregion
        }


    }
}
