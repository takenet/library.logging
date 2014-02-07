using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Takenet.Library.Logging.LogConsumer.Configuration
{
    /// <summary>
    /// Defines the log service configuration section
    /// </summary>
    public class LogConsumerConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Applications configuration
        /// </summary>
        [ConfigurationProperty("applications", IsRequired = true)]
        [ConfigurationCollection(typeof(LogConsumerApplicationElementCollection),
            AddItemName = "application")]
        public LogConsumerApplicationElementCollection ApplicationCollection
        {
            get { return (LogConsumerApplicationElementCollection)base["applications"]; }
        }

        /// <summary>
        /// Default name of section on configuration file
        /// </summary>
        public static string SectionName
        {
            get
            {
                return "logConsumerConfiguration";
            }
        }

        /// <summary>
        /// Gets the section
        /// </summary>
        public static LogConsumerConfiguration Section
        {
            get
            {
                return (LogConsumerConfiguration)ConfigurationManager.GetSection(SectionName);
            }
        }
    }

    #region LogConsumerApplicationElementCollection

    /// <summary>
    /// Application configuration collection
    /// </summary>
    public class LogConsumerApplicationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Type of configuration collection
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((LogConsumerApplicationElement)element).Name;
        }

        /// <summary>
        /// Returns a new element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogConsumerApplicationElement();
        }
    } 
    
    #endregion
    
    #region LogConsumerApplicationElement

    /// <summary>
    /// Represents a configuration element within a configuration file.
    /// </summary>
    public class LogConsumerApplicationElement : ConfigurationElement
    {
        /// <summary>
        /// Name of the application. 
        /// Can be used to get the queue path, if not specified on QueuePath configuration.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        /// <summary>
        /// Consumes from a specific queue path
        /// </summary>
        [ConfigurationProperty("queuePath", IsRequired = false)]
        public string QueuePath
        {
            get { return (string)this["queuePath"]; }
        }

        /// <summary>
        /// Number of consumer threads for this queue
        /// </summary>
        [ConfigurationProperty("consumersCount", IsRequired = false, DefaultValue = 1)]
        public int ConsumersCount
        {
            get { return (int)this["consumersCount"]; }
        }

        /// <summary>
        /// Type name of log listener
        /// </summary>
        [ConfigurationProperty("loggerType", IsRequired = true)]
        public string LoggerType
        {
            get { return (string)this["loggerType"]; }
        }

        /// <summary>
        /// Listener specific configurations
        /// </summary>
        [ConfigurationProperty("properties", IsRequired = false)]
        [ConfigurationCollection(typeof(LogConsumerApplicationElementCollection),
            AddItemName = "add")]
        public LogConsumerApplicationElementPropertyCollection PropertyCollection
        {
            get { return (LogConsumerApplicationElementPropertyCollection)base["properties"]; }
        }
    }


    #endregion

    #region LogConsumerApplicationElementPropertyCollection

    /// <summary>
    /// Represents a configuration element containing a collection of child elements.
    /// </summary>
    public class LogConsumerApplicationElementPropertyCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Type of configuration collection
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((LogConsumerApplicationPropertyElement)element).Name;
        }

        /// <summary>
        /// Returns a new element
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogConsumerApplicationPropertyElement();
        }
    }

    #endregion

    #region LogConsumerApplicationPropertyElement

    /// <summary>
    /// Listener specific configurations
    /// </summary>
    public class LogConsumerApplicationPropertyElement : ConfigurationElement
    {
        /// <summary>
        /// Name of configuration property
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        /// <summary>
        /// Value of configuration property
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true, IsKey = true)]
        public string Value
        {
            get { return (string)this["value"]; }
        }
    }

    #endregion
}
