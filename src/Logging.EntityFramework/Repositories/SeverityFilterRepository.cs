using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Takenet.Library.Data;
using Takenet.Library.Data.EntityFramework;
using Takenet.Library.Logging.Models;

namespace Takenet.Library.Logging.EntityFramework.Repositories
{
    public class SeverityFilterRepository : EntityRepositoryAsync<SeverityFilter, Guid>
    {
        public SeverityFilterRepository(IUnitOfWorkAsync unitOfWork)
            : base(unitOfWork as DbContext)
        {
        }
    }
}
