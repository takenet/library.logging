using Takenet.Library.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace Takenet.Library.Logging.Test
{       
    /// <summary>
    ///This is a test class for LogMessageTest and is intended
    ///to contain all LogMessageTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LogMessageTest
    {
        /// <summary>
        ///A test for LogMessage Constructor
        ///</summary>
        [TestMethod]
        public void LogMessageConstructorTest()
        {
            string title = "EnqueueLogTest";
            string message = "This is a test";
            string userName = "553199990000";
            TraceEventType severity = TraceEventType.Information;
            string applicationName = "LoggingTest";
            string[] categories = new[] { "Test", "Logging" };
            long correlationID = 0;

            Dictionary<string, string> extendedProperties = new Dictionary<string, string>();
            extendedProperties.Add("argument1", "value1");
            extendedProperties.Add("argument2", "value2");

            LogMessage logMessage = new LogMessage(title, message, userName, severity, applicationName, categories, correlationID, extendedProperties);

            Assert.AreEqual(logMessage.Title, title);
            Assert.AreEqual(logMessage.Message, message);
            Assert.AreEqual(logMessage.UserName, userName);
            Assert.AreEqual(logMessage.Severity, severity);
            Assert.AreEqual(logMessage.ApplicationName, applicationName);
            
            Assert.AreEqual(logMessage.Categories.Length, categories.Length);

            foreach (var category in categories)
            {
                Assert.IsTrue(logMessage.Categories.Any(c => c.Equals(category))); 
            }

            Assert.AreEqual(logMessage.CorrelationId, correlationID);
            Assert.AreEqual(logMessage.ExtendedProperties.Count, extendedProperties.Count);

            foreach (var propertyKey in extendedProperties.Keys)
            {
                Assert.IsTrue(logMessage.ExtendedProperties.ContainsKey(propertyKey));
                Assert.AreEqual(logMessage.ExtendedProperties[propertyKey], extendedProperties[propertyKey]);
            }
        }

        /// <summary>
        ///A test for LogMessage Constructor
        ///</summary>
        [TestMethod]
        public void LogMessageConstructorTest1()
        {            
            LogMessage logMessage = new LogMessage();

            Assert.IsNull(logMessage.Title);
            Assert.IsNull(logMessage.Message);
            Assert.AreEqual(logMessage.UserName, Environment.UserName);

            Assert.IsTrue(logMessage.Timestamp >= DateTime.UtcNow.AddSeconds(-5));
            Assert.IsTrue(logMessage.Timestamp <= DateTime.UtcNow);

            Assert.AreEqual(logMessage.Severity, TraceEventType.Verbose);
            Assert.IsNotNull(logMessage.ApplicationName);
            Assert.IsNotNull(logMessage.Categories);
            Assert.AreEqual(logMessage.CorrelationId, 0);
            Assert.IsNotNull(logMessage.ExtendedProperties);
        }
    }
}
