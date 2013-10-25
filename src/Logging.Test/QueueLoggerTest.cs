using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Takenet.Library.Logging;
using System.Messaging;
using System.Diagnostics;

namespace Takenet.Library.Logging.Test
{
    [TestClass]
    public class QueueLoggerTest
    {
        private string _applicationName;

        /// <summary>
        /// Delete the application queue if exists
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _applicationName = "LoggingTest";

            string queuePath = string.Format(QueueLogger.QUEUE_PATH_TEMPLATE, _applicationName);

            if (MessageQueue.Exists(queuePath))
            {
                MessageQueue.Delete(queuePath);
            }
        }

        /// <summary>
        /// Test both QueueLog and DequeueLog methods of QueueLogger class
        /// </summary>
        [TestMethod]
        public void WriteLogTest()
        {
            string title = "EnqueueLogTest";
            string message = "This is a test";
            string userName = "553199990000";
            TraceEventType severity = TraceEventType.Information;
            string[] categories = new[] { "Test", "Logging" };
            long correlationID = 0;

            Dictionary<string, string> extendedProperties = new Dictionary<string, string>();
            extendedProperties.Add("argument1", "value1");
            extendedProperties.Add("argument2", "value2");

            LogMessage logMessage = new LogMessage(title, message, userName, severity, _applicationName, categories, correlationID, extendedProperties);

            ILogger logger = new QueueLogger(null);
            logger.WriteLog(logMessage);

            LogMessage retrievedLogMessage = QueueLogger.DequeueLog(_applicationName);

            Assert.AreEqual(logMessage.ApplicationName, retrievedLogMessage.ApplicationName);
            Assert.AreEqual(logMessage.Categories.Length, retrievedLogMessage.Categories.Length);
            Assert.AreEqual(logMessage.CorrelationId, retrievedLogMessage.CorrelationId);
            Assert.AreEqual(logMessage.ExtendedProperties.Count, retrievedLogMessage.ExtendedProperties.Count);
            Assert.AreEqual(logMessage.MachineName, retrievedLogMessage.MachineName);
            Assert.AreEqual(logMessage.Message, retrievedLogMessage.Message);            
            Assert.AreEqual(logMessage.ProcessId, retrievedLogMessage.ProcessId);
            Assert.AreEqual(logMessage.ProcessName, retrievedLogMessage.ProcessName);
            Assert.AreEqual(logMessage.Severity, retrievedLogMessage.Severity);
            Assert.AreEqual(logMessage.ThreadId, retrievedLogMessage.ThreadId);
            Assert.AreEqual(logMessage.Timestamp, retrievedLogMessage.Timestamp);
            Assert.AreEqual(logMessage.Title, retrievedLogMessage.Title);
            Assert.AreEqual(logMessage.UserName, retrievedLogMessage.UserName);
        }
    }
}