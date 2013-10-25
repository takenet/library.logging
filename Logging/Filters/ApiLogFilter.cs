using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Takenet.Library.Logging.Models;

namespace Takenet.Library.Logging.Filters
{
    /// <summary>
    /// Obtains the filter conditions from the log Web API.
    /// Because the lag of HTTP calls, is recommended that the instances 
    /// of this class is wrapped by a CachedLogFilter instance.
    /// </summary>
    public class ApiLogFilter : ILogFilter
    {
        private string _baseAddress;
        private static DataContractJsonSerializer _severityFilterSerializer = new DataContractJsonSerializer(typeof(SeverityFilter));
        private static DataContractJsonSerializer _severityFilterArraySerializer = new DataContractJsonSerializer(typeof(SeverityFilter[]));
        
        #region Constructor
		
        /// <summary>
        /// Creates a new instance pointing
        /// to the specified log api address.
        /// </summary>
        /// <param name="baseAddress"></param>
        public ApiLogFilter(string baseAddress)
        {
            _baseAddress = baseAddress.TrimEnd('/');
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
            var shouldWrite = true;

            string filterAddress = string.Format("{0}/{1}/filters?machineName={2}&messageTitle={3}&categoryName={4}",
                _baseAddress, logMessage.ApplicationName, logMessage.MachineName, logMessage.Title, logMessage.Categories.FirstOrDefault());

            var request = HttpWebRequest.Create(filterAddress) as HttpWebRequest;
            request.Accept = "application/json";
            request.Method = "GET";

            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    SeverityFilter[] filterArray = null;

                    using (var responseStream = response.GetResponseStream())
                    {
                        filterArray = (SeverityFilter[])_severityFilterArraySerializer.ReadObject(responseStream);
                    }

                    if (filterArray.Any())
                    {
                        shouldWrite = logMessage.Severity <= filterArray.First().SeverityLevel;
                    }
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;

                if (response != null &&
                    response.StatusCode == HttpStatusCode.NotFound)
                {
                    var filter = new SeverityFilter();
                    filter.MachineName = logMessage.MachineName;
                    filter.MessageTitle = logMessage.Title;
                    filter.CategoryName = logMessage.Categories.FirstOrDefault();

                    string createFilterAddress = string.Format("{0}/{1}/filters",
                        _baseAddress, logMessage.ApplicationName);

                    var createRequest = HttpWebRequest.Create(createFilterAddress) as HttpWebRequest;
                    createRequest.ContentType = "application/json";
                    createRequest.Method = "POST";

                    using (var requestStream = createRequest.GetRequestStream())
                    {
                        _severityFilterSerializer.WriteObject(requestStream, filter);
                    }

                    using (var createResponse = createRequest.GetResponse() as HttpWebResponse)
                    {
                        if (createResponse.StatusCode != HttpStatusCode.OK)
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    throw;
                }
            }

            return shouldWrite;
        }

        #endregion
    }
}