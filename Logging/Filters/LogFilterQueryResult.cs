using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.Library.Logging.Filters
{
    /// <summary>
    /// The result of log query service
    /// for storing on local cache
    /// </summary>
    internal class LogFilterQueryResult
    {
        internal LogFilterQueryResult()
        {
            ResultDate = DateTime.Now;
        }


        /// <summary>
        /// Indicates the message to be logged or not
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Date of result 
        /// </summary>
        public DateTime ResultDate { get; set; }
    }
}
