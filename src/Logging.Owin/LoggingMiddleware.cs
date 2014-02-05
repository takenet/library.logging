using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.Library.Logging;

namespace Logging.Owin
{
    public class LoggingMiddleware : OwinMiddleware
    {
        private const string CORRELATIONHEADERKEY = "X-CorrelationId";

        public LoggingMiddleware(OwinMiddleware next, IAppBuilder app, LoggingMiddlewareOptions options)
            : base(next)
        {
            Options = options;
        }

        public LoggingMiddlewareOptions Options { get; set; }

        public override async Task Invoke(IOwinContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            long correlationId;

            if (context.Request.Headers.Keys.Contains(CORRELATIONHEADERKEY))
            {
                var correlation = context.Request.Headers.Get(CORRELATIONHEADERKEY);
                
                if (!long.TryParse(correlation, out correlationId))
                {
                    correlationId = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
                }

            }
            else
            {
                correlationId = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
            }

            string content = string.Empty;
            using (var receiveStream = context.Request.Body)
            {
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    content = readStream.ReadToEnd();
                }
            }

            var extendedProperties = new Dictionary<string, string>() { { context.Request.ContentType, content.Replace("=", ":") } };

            var logMessage = new LogMessage
            {
                UserName = context.Request.User != null ? context.Request.User.Identity.Name : string.Empty,
                Title = "BeginRequest",
                Categories = new[] { "LoggingOwin" },
                ApplicationName = "",
                Message = string.Format("Begin request on Uri: {0} Method: {1}",
                                context.Request.Uri.ToString(), context.Request.Method),
                ExtendedProperties = extendedProperties,
                CorrelationId = correlationId
            };

            try
            {
                LogMessage(logMessage);
                await Next.Invoke(context);
            }
            catch(Exception ex)
            {
                logMessage.Message = ex.Message;
                logMessage.Title = "ErrorRequest";
                logMessage.Timestamp = DateTime.UtcNow;
                LogMessage(logMessage);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                logMessage.Message = string.Format("End request on Uri: {0} Method: {1} Elapsed: {2}", context.Request.Uri.ToString(), context.Request.Method, stopwatch.ElapsedMilliseconds);
                logMessage.Title = "EndRequest";
                logMessage.Timestamp = DateTime.UtcNow;
                LogMessage(logMessage);
            }
        }


        private void LogMessage(LogMessage logMessage)
        {
            if(Options.LogFilter.ShouldWriteLog(logMessage))
            {
                Options.Logger.WriteLog(logMessage);
            }
        }
    }
}
