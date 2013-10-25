using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Takenet.Library.Logging.Filters
{
    /// <summary>
    /// Represents a query to the log filter service
    /// </summary>
    [DataContract]
    [Serializable]
    public class LogFilterQuery
    {
        /// <summary>
        /// Log message title
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Log message severity
        /// </summary>
        [DataMember]
        public TraceEventType Severity { get; set; }

        /// <summary>
        /// Name of application which called the logger
        /// </summary>
        [DataMember]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Log message machine name
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }

        /// <summary>
        /// Categories of log message
        /// </summary>
        [DataMember]
        public string[] Categories { get; set; }

        #region Public methods

        /// <summary>
        /// Defines a new hash code to cache dictionary
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Defina a comparison method to cache dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            LogFilterQuery otherFilterQuery = (LogFilterQuery)obj;
            return ToString().Equals(otherFilterQuery.ToString());
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder hashBuilder = new StringBuilder();
            hashBuilder.AppendFormat("{0}.{1}.{2}.{3}.", Title, Severity, ApplicationName, MachineName);

            if (Categories != null)
            {
                foreach (var category in Categories)
                {
                    hashBuilder.AppendFormat("{0}.", category);
                }
            }

            return hashBuilder.ToString().TrimEnd('.');
        }

        #endregion
    }
}
