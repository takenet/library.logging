using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Takenet.Library.Logging.Filters;
using System.Diagnostics;
using System.Threading;

namespace Takenet.Library.Logging.Test
{
    [TestClass]
    public class ServiceLogFilterTest
    {
        private string _applicationName;

        /// <summary>
        /// Delete the application queue if exists
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _applicationName = "LoggingTest";
        }

        [TestMethod]
        public void ServiceLogFilterConstructorTest()
        {
            ServiceLogFilter filter = new ServiceLogFilter();

            //string title = "EnqueueLogTest";
            //string message = "This is a test";
            //string userName = "553199990000";
            //TraceEventType severity = TraceEventType.Information;
            //string[] categories = new[] { "Test", "Logging" };
            //long correlationID = 0;

            //Dictionary<string, string> extendedProperties = new Dictionary<string, string>();
            //extendedProperties.Add("argument1", "value1");
            //extendedProperties.Add("argument2", "value2");

            //LogMessage logMessage = new LogMessage(title, message, userName, severity, _applicationName, categories, correlationID, extendedProperties);

            //bool result = filter.ShouldWriteLog(logMessage);

            //Thread.Sleep(TimeSpan.FromSeconds(15));

            //result = filter.ShouldWriteLog(logMessage);

            //Thread.Sleep(TimeSpan.FromSeconds(10));
        }
    }
}
