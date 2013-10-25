using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Defines a simple and common log interface.
    /// </summary>
    public interface ILoggerAsync
    {
        /// <summary>
        /// Provides a filter for log messages
        /// </summary>
        ILogFilter Filter { get; }

        /// <summary>
        /// Writes a logs a message
        /// </summary>
        /// <param name="logMessage">The message to be logged</param>
        Task WriteLogAsync(LogMessage logMessage);
    }
}
