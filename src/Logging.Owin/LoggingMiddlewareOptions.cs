using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Library.Logging;

namespace Takenet.Library.Logging.Owin
{
    public class LoggingMiddlewareOptions
    {
        public LoggingMiddlewareOptions(ILogger logger, ILogFilter logFilter)
        {
            Logger = logger;
            LogFilter = logFilter;
        }

        public ILogFilter LogFilter { get; set; }

        public ILogger Logger { get; set; }
    }
}
