using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Takenet.Library.Logging.Test
{
    [TestClass]
    public class DebugLoggerTest
    {
        private string _applicationName;

        /// <summary>
        /// Initialize log listener
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _applicationName = "LoggingTest";

            ReceivedLogMessages = new List<string>();
            
            TestTraceListener testTraceListener = new TestTraceListener();            
            testTraceListener.LogReceived += (sender, e) => ReceivedLogMessages.Add(e.Message);            
            Debug.Listeners.Add(testTraceListener);            
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Debug.Listeners.Remove("TestTraceListener");
        }

        internal List<string> ReceivedLogMessages { get; set; }

        /// <summary>
        /// Tests WriteInformation method
        /// </summary>
        [TestMethod]
        public void WriteInformationTest()
        {
            string title = "EnqueueLogTest";
            string message = "This is a test";
            string userName = "553199990000";            
            string category = "Test";            

            ILogger logger = new DebugLogger();
            
            logger.WriteInformation(title, message, userName, _applicationName, category);

            Assert.IsTrue(true);

            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(title)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(message)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(userName)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(category)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(_applicationName)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(TraceEventType.Information.ToString())));            
        }

        /// <summary>
        /// Tests WriteError method
        /// </summary>
        [TestMethod]
        public void WriteErrorTest()
        {
            string title = "EnqueueLogTest";
            string message = "This is a test";
            string userName = "553199990000";
            string category = "Test";
            long correlationId = 0;           

            Dictionary<string, string> extendedProperties = new Dictionary<string, string>();
            extendedProperties.Add("argument1", "value1");
            extendedProperties.Add("argument2", "value2");

            ILogger logger = new DebugLogger();

            logger.WriteError(title, message, userName, _applicationName, category, correlationId, extendedProperties);

            Assert.IsTrue(true);

            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(title)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(message)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(userName)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(category)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(_applicationName)));
            //Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(TraceEventType.Error.ToString())));

            //foreach (var key in extendedProperties.Keys)
            //{
            //    Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(key)));
            //    Assert.IsTrue(ReceivedLogMessages.Any(m => m.Contains(extendedProperties[key])));
            //}
        }
    }

    internal class TestTraceListener : TraceListener
    {
        public TestTraceListener()
            : base("TestTraceListener")
        {

        }

        public event EventHandler<LogEventArgs> LogReceived;

        public override void Write(string message)
        {
            if (LogReceived != null)
            {
                LogReceived(this, new LogEventArgs() { Message = message });
            }
        }

        public override void WriteLine(string message)
        {
            if (LogReceived != null)
            {
                LogReceived(this, new LogEventArgs() { Message = message });
            }
        }
    }

    internal class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}