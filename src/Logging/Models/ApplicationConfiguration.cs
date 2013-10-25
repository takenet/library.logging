using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Takenet.Library.Logging.Models
{
    /// <summary>
    /// Settings for application logging, such filters
    /// and repository configuration
    /// </summary>
    [Serializable]
    public class ApplicationConfiguration
    {
        /// <summary>
        /// Creates a new instance with default settings
        /// </summary>
        public ApplicationConfiguration()
        {
            ApplicationConfigurationId = Guid.NewGuid();            
            SeverityLevel = TraceEventType.Warning;
            SeverityFilterCollection = new List<SeverityFilter>();
        }

        /// <summary>
        /// Unique identifier for application configuration
        /// </summary>
        public Guid ApplicationConfigurationId { get; set; }

        private string _applicationName;

        /// <summary>
        /// Name of application for logging
        /// </summary>
        public string ApplicationName
        {
            get { return _applicationName; }
            set 
            { 
                _applicationName = value;
                LogRepositoryName = string.Format("{0}Logs", _applicationName);
            }
        }

        /// <summary>
        /// Name of log repository for current application.
        /// Can be a table, file or collection name, for instance.
        /// </summary>
        public string LogRepositoryName { get; set; }

        /// <summary>
        /// Maximum severity level for logging
        /// </summary>
        public TraceEventType SeverityLevel { get; set; }

        public int SeverityLevelFlat
        {
            get { return (int)SeverityLevel; }
            set { SeverityLevel = (TraceEventType)value; }
        }

        /// <summary>
        /// Collection of severity filters with conditions to filter the application log
        /// </summary>
        public virtual ICollection<SeverityFilter> SeverityFilterCollection { get; set; }
    }
}
