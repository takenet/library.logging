﻿using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Takenet.Library.Logging.Owin;

namespace Owin
{
    public static class LoggingMiddlewareExtensions
    {
        public static IAppBuilder UseLogging(this IAppBuilder app, LoggingMiddlewareOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            app.Use(typeof(LoggingMiddleware), app, options);
            return app;
        }
    }
}
