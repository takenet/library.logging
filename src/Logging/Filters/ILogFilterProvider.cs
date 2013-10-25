using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;

namespace Takenet.Library.Logging.Filters
{
    [ServiceContract]
    public interface ILogFilterProvider
    {
        [OperationContract]
        bool ShouldWriteLog(LogFilterQuery logFilterQuery);
    }
}