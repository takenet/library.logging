using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Takenet.Library.Logging.Models;

namespace Takenet.Library.Logging.EntityFramework.Configurations
{
    public class SeverityFilterConfiguration : EntityTypeConfiguration<SeverityFilter>
    {
        public SeverityFilterConfiguration()
        {
            this.HasKey(e => e.SeverityFilterId)
                .Map(e => e.ToTable("TbSeverityFilter", "dbo"));

            this.Property(e => e.CategoryName)
                .HasMaxLength(50);

            this.Property(e => e.MachineName)
                .HasMaxLength(50);

            this.Property(e => e.MessageTitle)
                .HasMaxLength(100);

            this.Ignore(e => e.SeverityLevel);
            this.Ignore(e => e.SeverityLevelFlatString);
            this.Property(e => e.SeverityLevelFlat)
                .HasColumnName("SeverityLevel");
                
            this.HasRequired(e => e.ApplicationConfiguration)
                .WithMany(e => e.SeverityFilterCollection)
                .HasForeignKey(e => e.ApplicationConfigurationId)
                .WillCascadeOnDelete(false);
        }
    }
}
