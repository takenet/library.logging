using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Takenet.Library.Logging.Loggers;

namespace Takenet.Library.Logging.Test
{
    [TestClass]
    public class TextFileLoggerTest
    {
        private string _applicationName;

        /// <summary>
        /// Initialize log listener
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _applicationName = "LoggingTest";
        }

        /// <summary>
        /// Tests WriteInformation method
        /// </summary>
        [TestMethod]
        public void WriteErrorTest()
        {
            string title = "EnqueueLogTest";
            string message = "This is a test";
            string userName = "553199990000";
            string category = "Test";

            ILogger logger = new TextFileLogger();

            Dictionary<string, string> extendedProperties = new Dictionary<string, string>();
            extendedProperties.Add("argument1", "value1");
            extendedProperties.Add("argument2", "value2");            

            logger.WriteError(title, message, userName, _applicationName, category, 0, extendedProperties);

            Assert.IsTrue(true);
        }
    }
}
