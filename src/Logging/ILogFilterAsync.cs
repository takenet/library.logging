using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Defines a log filter
    /// </summary>
    public interface ILogFilterAsync
    {
        /// <summary>
        /// Asynchronously checks if a specific entry should be logged.
        /// </summary>
        /// <param name="logMessage">The log entry to be checked.</param>
        /// <returns></returns>
        Task<bool> ShouldWriteLogAsync(LogMessage logMessage);
    }
}
