using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Collections.Concurrent;
using System.ServiceModel.Description;
using System.Timers;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Net;

namespace Takenet.Library.Logging.Filters
{
    /// <summary>
    /// Provides a log filter that obtains
    /// the filter conditions on a service in current network.
    /// The service is discovered using WS-Discovery protocol.
    /// </summary>
    [Serializable]
    public class ServiceLogFilter : ILogFilter
    {
        #region Private fields

        public static event EventHandler<ServiceCacheEventArgs> ServiceCacheErrorOccured;

        private static ChannelFactory<ILogFilterProvider> _logFilterProviderChannelFactory;
        private static object _syncRoot = new object();
        private static Timer _cacheTimer;
        private static Queue<LogFilterQuery> _pendingQueriesQueue = new Queue<LogFilterQuery>();

        private static ObjectCache _logFilterCache = new MemoryCache("LogFilterCache");

        private const int PENDING_QUEUE_LIMIT = 100;

#if DEBUG
        private static readonly TimeSpan _cacheValidity = TimeSpan.FromSeconds(10);
#else
        private static readonly TimeSpan _cacheValidity = TimeSpan.FromMinutes(3); 
#endif

        #endregion

        #region Constructor

        /// <summary>
        /// Starts static elements of filter
        /// </summary>
        static ServiceLogFilter()
        {
            _cacheTimer = new Timer(_cacheValidity.TotalMilliseconds);
            _cacheTimer.Elapsed += new ElapsedEventHandler(CacheTimer_Elapsed);
            _cacheTimer.Start();
        }

        /// <summary>
        /// Creates a new log filter instance with a
        /// default alternative log filter
        /// </summary>
        public ServiceLogFilter()
            : this(new SeverityLogFilter())
        {

        }

        /// <summary>
        /// Creates a new log filter instance using a
        /// specific alternative log filter
        /// </summary>
        /// <param name="alternativeLogFilter">Alternative filter for the cases when the log filter service is not available</param>
        public ServiceLogFilter(ILogFilter alternativeLogFilter)
        {
            AlternativeLogFilter = alternativeLogFilter;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Indicates a alternative log filter to be used
        /// in cases when the service log filter fails
        /// to determine if a message should be logged
        /// </summary>
        public ILogFilter AlternativeLogFilter { get; private set; }

        #endregion

        #region ILogFilter Members

        /// <summary>
        /// Checks if a specific entry should be logged.
        /// </summary>
        /// <param name="severity">Level of severity of log message</param>
        /// <param name="applicationName">Name of current application</param>
        /// <param name="machineName">Name of the machine where the process is running</param>
        /// <param name="categories">Categories where the current log message fits</param>
        /// <returns>A boolean value indicatin if the message should be logged or not</returns>
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

                LogFilterQueryResult queryResult = _logFilterCache.Get(filterQuery.ToString().ToLower()) as LogFilterQueryResult;

                if (queryResult != null)
                {
                    result = queryResult.Result;
                }
                else
                {
                    if (_pendingQueriesQueue.Count < PENDING_QUEUE_LIMIT &&
                        !_pendingQueriesQueue.Contains(filterQuery))
                    {
                        _pendingQueriesQueue.Enqueue(filterQuery);
                    }

                    if (AlternativeLogFilter != null)
                    {
                        result = AlternativeLogFilter.ShouldWriteLog(logMessage);
                    }
                }
            }
            catch
            {
                if (AlternativeLogFilter != null)
                {
                    result = AlternativeLogFilter.ShouldWriteLog(logMessage);
                }
                else
                {
                    throw;
                }
            }

            // Filter cache
            logMessage.ShouldWriteLog = result;

            return result;
        }

        #endregion

        #region Private static methods

        private static void CacheTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _cacheTimer.Enabled = false;

            try
            {
                if (_pendingQueriesQueue.Count > 0)
                {
                    TryCreateChannelFactory();

                    if (_logFilterProviderChannelFactory != null)
                    {
                        ILogFilterProvider client = _logFilterProviderChannelFactory.CreateChannel();

                        try
                        {
                            // Proccess the pending items
                            while (_pendingQueriesQueue.Count > 0)
                            {
                                var filterQuery = _pendingQueriesQueue.Dequeue();

                                if (!_logFilterCache.Contains(filterQuery.ToString()))
                                {
                                    LogFilterQueryResult queryResult = new LogFilterQueryResult()
                                    {
                                        Result = client.ShouldWriteLog(filterQuery),
                                        ResultDate = DateTime.Now
                                    };

                                    _logFilterCache.Add(filterQuery.ToString().ToLower(), queryResult, new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.Add(_cacheValidity) });
                                }
                            }
                        }
                        catch (EndpointNotFoundException)
                        {
                            // The discovered endpoint is now invalid
                            try
                            {
                                _logFilterProviderChannelFactory.Close();
                            }
                            finally
                            {
                                _logFilterProviderChannelFactory = null;
                            }
                        }
                        finally
                        {
                            ((IDisposable)client).Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ServiceCacheErrorOccured != null)
                {
                    ServiceCacheErrorOccured(null, new ServiceCacheEventArgs(ex));
                }
            }
            finally
            {
                _cacheTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Try to discover an endpoint to the
        /// log filter requests
        /// </summary>
        private static void TryCreateChannelFactory()
        {
            if (_logFilterProviderChannelFactory == null)
            {
                lock (_syncRoot)
                {
                    if (_logFilterProviderChannelFactory == null)
                    {
                        // Try to find any service who implements ILogFilterProvider
                        // using WS-Discovery
                        DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                        FindResponse response = discoveryClient.Find(new FindCriteria(typeof(ILogFilterProvider)));

                        // If any endpoint is found, instantiate the channel factory
                        if (response.Endpoints.Count > 0)
                        {
                            // Gives the preference to a local machine endpoint, if exists
                            var localEndpoint = response.Endpoints.FirstOrDefault(e => IsLocalIpAddress(e.Address.Uri.Host));
                            
                            EndpointAddress endpointAddress;

                            if (localEndpoint != null)
                            {
                                endpointAddress = localEndpoint.Address;
                            }
                            else
                            {
                                endpointAddress = response.Endpoints.First().Address;
                            }
                           
                            var binding = new NetTcpBinding(SecurityMode.None);
                            var serviceEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(ILogFilterProvider)),
                                                                        binding,
                                                                        endpointAddress);

                            _logFilterProviderChannelFactory = new ChannelFactory<ILogFilterProvider>(serviceEndpoint);
                        }
                    }
                }
            }
        }

        private static bool IsLocalIpAddress(string host)
        {
            try
            { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }
        #endregion
    }
}