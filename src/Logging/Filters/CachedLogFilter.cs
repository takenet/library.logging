using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.Library.Logging.Filters
{
    /// <summary>
    /// Provides a basic cache structure for
    /// log filtering
    /// </summary>
    public class CachedLogFilter : ILogFilter
    {
        private readonly ILogFilter _logFilter;
        private readonly ILogFilter _alternativeLogFilter;
        private TimeSpan _cacheValidity;

        private ObjectCache _logFilterCache = new MemoryCache("CachedLogFilter");

        #region Constructor

        /// <summary>
        /// Creates a new instance with 
        /// default cache options 
        /// and a verbose log filter as alternative
        /// </summary>
        /// <param name="logFilter">Log filter to be wrapper</param>
        public CachedLogFilter(ILogFilter logFilter)
            : this(logFilter, TimeSpan.FromMinutes(5))
        {

        }

        /// <summary>
        /// Creates a new instance with 
        /// specified cache options
        /// and a verbose log filter as alternative
        /// </summary>
        /// <param name="logFilter">Log filter to be wrapper</param>
        /// <param name="cacheValidity">Validity of the wrapper log filter values on cache</param>
        public CachedLogFilter(ILogFilter logFilter, TimeSpan cacheValidity)
            : this(logFilter, cacheValidity, new SeverityLogFilter(System.Diagnostics.TraceEventType.Verbose))
        {

        }

        /// <summary>
        /// Creates a new instance with 
        /// specified cache options
        /// and the specified log filter as alternative
        /// </summary>
        /// <param name="logFilter"></param>
        /// <param name="cacheValidity">Validity of the wrapper log filter values on cache</param>
        /// <param name="alternativeLogFilter">Filter to be used while the main log filter values are not available</param>
        public CachedLogFilter(ILogFilter logFilter, TimeSpan cacheValidity, ILogFilter alternativeLogFilter)
        {
            if (logFilter == null)
            {
                throw new ArgumentNullException("logFilter");
            }

            if (alternativeLogFilter == null)
            {
                throw new ArgumentNullException("alternativeLogFilter");
            }

            _logFilter = logFilter;
            _alternativeLogFilter = alternativeLogFilter;
            _cacheValidity = cacheValidity;
        }

        #endregion

        #region ILogFilter Members

        /// <summary>
        /// Checks if a specific entry should be logged.
        /// </summary>
        /// <param name="logMessage"></param>
        /// <returns></returns>
        public bool ShouldWriteLog(LogMessage logMessage)
        {
            bool result = true;

            try
            {
                var filterQuery = new LogFilterQuery()
                {
                    ApplicationName = logMessage.ApplicationName,
                    Severity = logMessage.Severity,
                    MachineName = logMessage.MachineName,
                    Title = logMessage.Title,
                    Categories = logMessage.Categories
                };

                int filterQueryHash = filterQuery.GetHashCode();

                LogFilterQueryResult queryResult = _logFilterCache.Get(
                    filterQuery.ToString().ToLower()) as LogFilterQueryResult;

                if (queryResult != null)
                {
                    result = queryResult.Result;
                }
                else
                {
                    var shouldWriteTask = Task.Factory.StartNew(() =>
                    {
                        queryResult = new LogFilterQueryResult
                        {
                            Result = _logFilter.ShouldWriteLog(logMessage),
                            ResultDate = DateTime.Now
                        };

                        _logFilterCache.Add(
                            filterQuery.ToString().ToLower(), 
                            queryResult, 
                            new CacheItemPolicy() 
                            { 
                                AbsoluteExpiration = DateTimeOffset.Now.Add(_cacheValidity) 
                            });
                    });

                    result = _alternativeLogFilter.ShouldWriteLog(logMessage);
                }
            }
            catch
            {
                result = _alternativeLogFilter.ShouldWriteLog(logMessage);
            }

            // Filter cache
            logMessage.ShouldWriteLog = result;

            return result;
        }

        #endregion
    }
}