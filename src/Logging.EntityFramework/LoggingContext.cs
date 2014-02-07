using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Takenet.Library.Logging.EntityFramework.Configurations;
using Takenet.Library.Data.EntityFramework;
using Takenet.Library.Logging;
using Takenet.Library.Logging.Models;

namespace Takenet.Library.Logging.EntityFramework
{
    public class LoggingContext : UnitOfWorkDbContext
    {
        private const string LOGGING_CONNECTION_STRING_NAME = "LoggingConnectionString";

#if DEBUG
        public readonly Guid InstanceId = Guid.NewGuid(); 
#endif

        #region Constructor

        public LoggingContext()
            : this(string.Format("name={0}", LOGGING_CONNECTION_STRING_NAME))
        {

        }

        public LoggingContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        #endregion

        public DbSet<LogMessage> LogMessageSet { get; set; }

        public DbSet<ApplicationConfiguration> ApplicationConfigurationSet { get; set; }

        public DbSet<SeverityFilter> SeverityFilterSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LogMessageConfiguration());
            modelBuilder.Configurations.Add(new ApplicationConfigurationConfiguration());
            modelBuilder.Configurations.Add(new SeverityFilterConfiguration());                
        }
    }
}
