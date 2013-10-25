using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Takenet.Library.Logging.Models
{
    /// <summary>
    /// Represents a combination of conditions to be 
    /// filtered by a severity
    /// </summary>
    [Serializable]
    [DataContract]
    public class SeverityFilter
    {
        public SeverityFilter()
        {
            SeverityFilterId = Guid.NewGuid();
            SeverityLevel = TraceEventType.Warning;            
        }

        [DataMember(EmitDefaultValue = false)]        
        public Guid SeverityFilterId { get; set; }

        /// <summary>
        /// Related Category
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; }

        /// <summary>
        /// Related Machine
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }
        
        /// <summary>
        /// Related MessageTitle
        /// </summary>
        [DataMember]
        public string MessageTitle { get; set; }

        /// <summary>
        /// Related ApplicationConfiguration
        /// </summary>
        [IgnoreDataMember]
        public virtual ApplicationConfiguration ApplicationConfiguration { get; set; }
        /// <summary>
        /// Related ApplicationConfiguration identifier
        /// </summary>
        [DataMember(EmitDefaultValue = false)]  
        public Guid ApplicationConfigurationId { get; set; }

        [IgnoreDataMember]
        public virtual TraceEventType SeverityLevel { get; set; }

        [IgnoreDataMember]        
        public int SeverityLevelFlat
        {
            get { return (int)SeverityLevel; }
            set { SeverityLevel = (TraceEventType)value; }
        }

        [DataMember(Name = "SeverityLevel")]
        public string SeverityLevelFlatString
        {
            get { return Enum.GetName(typeof(TraceEventType), SeverityLevel); }
            set { SeverityLevel = (TraceEventType)Enum.Parse(typeof(TraceEventType), value); }
        }
    }
}