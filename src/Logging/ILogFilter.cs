using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Defines a log filter
    /// </summary>
    public interface ILogFilter
    {
        /// <summary>
        /// Checks if a specific entry should be logged.
        /// </summary>
        /// <param name="severity">Level of severity of log message</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="machineName">Name of the machine where the process is running</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <returns></returns>
        bool ShouldWriteLog(LogMessage logMessage);
    }
}