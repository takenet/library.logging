using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Takenet.Library.Data;
using Takenet.Library.Logging.Models;

namespace Takenet.Library.Logging.EntityFramework
{
    public interface ILoggingContext : IUnitOfWorkAsync
    {
        IEntityRepositoryAsync<LogMessage, long> LogMessageRepository { get; }

        IEntityRepositoryAsync<ApplicationConfiguration, Guid> ApplicationConfigurationRepository { get; }

        IEntityRepositoryAsync<SeverityFilter, Guid> SeverityFilterRepository { get; }
    }
}
