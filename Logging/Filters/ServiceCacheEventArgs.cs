using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.Library.Logging.Filters
{
    /// <summary>
    /// Represents an error data with service filter cache
    /// </summary>
    public class ServiceCacheEventArgs : EventArgs
    {
        /// <summary>
        /// Instantiate a new args
        /// </summary>
        /// <param name="ex"></param>
        public ServiceCacheEventArgs(Exception ex)
        {            
            ServiceException = ex;
        }

        /// <summary>
        /// The exception that is not handled
        /// </summary>
        public Exception ServiceException { get; set; }
    }
}
