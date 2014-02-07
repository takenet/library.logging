using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Takenet.Library.Logging.Http;

namespace Takenet.Library.Logging.LogConsumer.Factories
{
    public class HttpLogFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        public ILogger Create(string applicationName, IDictionary<string, string> propertyDictionary)
        {
            var url = propertyDictionary["url"];

            return new HttpLogger(new Uri(url));
        }

        public Type LoggerType
        {
            get { return typeof(HttpLogFactory); }
        }

        #endregion
    }
}
