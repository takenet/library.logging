using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Common interface for log listeners
    /// </summary>
    public interface ILoggerFactory
    {
        ILogger Create(string applicationName, IDictionary<string, string> propertyDictionary);

        Type LoggerType { get; }
    }
}
