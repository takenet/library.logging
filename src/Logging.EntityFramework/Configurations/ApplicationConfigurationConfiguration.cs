using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Takenet.Library.Logging.Models;

namespace Takenet.Library.Logging.EntityFramework.Configurations
{
    public class ApplicationConfigurationConfiguration : EntityTypeConfiguration<ApplicationConfiguration>
    {
        public ApplicationConfigurationConfiguration()
        {
            this.HasKey(e => e.ApplicationConfigurationId)
                .Map(e => e.ToTable("TbApplicationConfiguration", "dbo"));

            this.Property(e => e.ApplicationName)
                .HasMaxLength(50);

            this.Property(e => e.LogRepositoryName)
                .HasMaxLength(50);

            this.Ignore(e => e.SeverityLevel);
            this.Property(e => e.SeverityLevelFlat)
                .HasColumnName("SeverityLevel");
        }
    }
}
