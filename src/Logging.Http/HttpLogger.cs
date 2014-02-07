using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.Library.Logging.Http
{
     /// <summary>
     /// Logs the messages to a specific
     /// HTTP server
     /// </summary>
    public class HttpLogger : ILogger, ILoggerAsync, IDisposable
    {
        #region Private fields

        private Uri _requestUri;
        private HttpClient _httpClient;
        private HttpMethod _method;
        private bool _disposed;

        #endregion

        #region Constructors

        public HttpLogger(Uri requestUri)
            : this(requestUri, null)
        {

        }

        public HttpLogger(Uri requestUri, ILogFilter filter)
            : this(requestUri, HttpMethod.Post, filter)
        {

        }

        public HttpLogger(Uri requestUri, HttpMethod method, ILogFilter filter)
        {
            _requestUri = requestUri;

            if (method != HttpMethod.Post &&
                method != HttpMethod.Put)
            {
                throw new ArgumentException("Only POST and PUT methods are supported", "method");
            }

            _method = method;
            _httpClient = new HttpClient();

            Filter = filter;
        }

        ~HttpLogger()
        {
            Dispose(false);
        }

        #endregion

        #region ILogger Members

        public ILogFilter Filter { get; private set; }

        public void WriteLog(LogMessage logMessage)
        {
            try
            {
                WriteLogAsync(logMessage).Wait();            
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        #endregion

        #region ILoggerAsync Members

        public async Task WriteLogAsync(LogMessage logMessage)
        {
            var serializedMessage = Serialize(logMessage);

            HttpResponseMessage responseMessage;

            if (_method == HttpMethod.Put)
            {
                responseMessage = await _httpClient.PutAsync(
                    _requestUri,
                    new StringContent(
                        serializedMessage,
                        this.Encoding,
                        this.MediaType)
                )
                .ConfigureAwait(false);
            }
            else
            {
                responseMessage = await _httpClient.PostAsync(
                    _requestUri,
                    new StringContent(
                        serializedMessage,
                        this.Encoding,
                        this.MediaType)
                )
                .ConfigureAwait(false);
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new HttpRequestException(responseMessage.ReasonPhrase);
            }
        }

        #endregion

        #region Protected Members

        protected virtual string Serialize(LogMessage logMessage)
        {
            return TypeSerializer.SerializeToString(logMessage);
        }

        protected virtual Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        protected virtual string MediaType
        {
            get { return "application/json"; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);            
        }        

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
