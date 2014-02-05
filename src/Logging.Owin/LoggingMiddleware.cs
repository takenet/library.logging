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

namespace Takenet.Library.Logging.Owin
{
    public class LoggingMiddleware : OwinMiddleware
    {
        public const string CORRELATION_HEADER_KEY = "X-CorrelationId";
        public const string CONTENT_KEY = "X-Content";

        public LoggingMiddleware(OwinMiddleware next, IAppBuilder app, LoggingMiddlewareOptions options)
            : base(next)
        {
            _options = options;
        }

        private static LoggingMiddlewareOptions _options { get; set; }

        public static void LogMessage(IOwinContext context, LogMessage logMessage)
        {
            if (_options.LogFilter.ShouldWriteLog(logMessage))
            {
                SetLogMessageVariables(context, logMessage);
                _options.Logger.WriteLog(logMessage);
            }
        }

        public override async Task Invoke(IOwinContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            long correlationId;

            if (context.Request.Headers.Keys.Contains(CORRELATION_HEADER_KEY))
            {
                var correlation = context.Request.Headers.Get(CORRELATION_HEADER_KEY);

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

            context.Request.Body.Position = 0;
            context.Set<long>(CORRELATION_HEADER_KEY, correlationId);
            context.Set<string>(CONTENT_KEY, content);

            var logMessage = new LogMessage
            {
                Title = "BeginRequest",
                Categories = new[] { "LoggingOwin" },
                Message = string.Format("Uri: {0} Method: {1}",
                                context.Request.Uri.ToString(), context.Request.Method),
                Severity = TraceEventType.Verbose
            };

            try
            {
                LogMessage(context, logMessage);
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                logMessage.Message = string.Format("Uri: {0} Method: {1} Message: {2}", context.Request.Uri.ToString(), context.Request.Method, ex.Message);
                logMessage.Title = "ErrorRequest";
                logMessage.Timestamp = DateTime.UtcNow;
                LogMessage(context, logMessage);
                throw;
            }
            finally
            {
                stopwatch.Stop();
                logMessage.Message = string.Format("Uri: {0} Method: {1} Elapsed: {2}", context.Request.Uri.ToString(), context.Request.Method, stopwatch.ElapsedMilliseconds);
                logMessage.Title = "EndRequest";
                logMessage.Timestamp = DateTime.UtcNow;
                LogMessage(context, logMessage);
            }
        }

        private static void SetLogMessageVariables(IOwinContext context, LogMessage logMessage)
        {
            var extendedProperties = new Dictionary<string, string>() { { context.Request.ContentType ?? string.Empty, context.Get<string>(CONTENT_KEY).Replace("=", ":") } };
            logMessage.UserName = context.Request.User != null ? context.Request.User.Identity.Name : string.Empty;
            logMessage.ApplicationName = context.Environment["host.AppName"].ToString();
            logMessage.ExtendedProperties = extendedProperties;
            logMessage.CorrelationId = context.Get<long>(CORRELATION_HEADER_KEY);
        }
    }
}
