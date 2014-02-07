using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using Takenet.Library.Logging;

namespace Takenet.Library.Logging.EntityFramework.Configurations
{
    public class LogMessageConfiguration : EntityTypeConfiguration<LogMessage>
    {
        internal const int APPLICATIONNAME_LENGTH = 50;
        internal const int MACHINENAME_LENGTH = 50;
        internal const int PROCESSNAME_LENGTH = 50;
        internal const int USERNAME_LENGTH = 50;
        internal const int MESSAGE_LENGTH = 1000;
        internal const int TITLE_LENGTH = 100;
        internal const int CATEGORIES_LENGTH = 100;
        internal const int EXTENDEDPROPERTIES_LENGTH = 500;

        public LogMessageConfiguration()
        {
            this.HasKey(e => e.LogMessageSafeId)
                .Map(e => e.ToTable("TbLogMessage", "dbo"));

            this.Ignore(e => e.LogMessageId);
            this.Property(e => e.LogMessageSafeId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("LogMessageId");

            this.Ignore(e => e.Severity);
            this.Property(e => e.SeverityFlat)
                .HasColumnName("Severity");
                
            this.Property(e => e.ApplicationName)
                .HasMaxLength(APPLICATIONNAME_LENGTH);

            this.Property(e => e.MachineName)
                .HasMaxLength(MACHINENAME_LENGTH);

            this.Property(e => e.Message)
                .HasMaxLength(MESSAGE_LENGTH);

            this.Property(e => e.Title)
                .HasMaxLength(TITLE_LENGTH);

            this.Property(e => e.ProcessName)
                .HasMaxLength(PROCESSNAME_LENGTH);

            this.Property(e => e.UserName)
                .HasMaxLength(USERNAME_LENGTH);

            this.Ignore(e => e.Categories);
            this.Property(e => e.CategoriesFlat)
                .HasColumnName("Categories")
                .HasMaxLength(CATEGORIES_LENGTH);

            this.Ignore(e => e.ExtendedProperties);
            this.Property(e => e.ExtendedPropertiesFlat)
                .HasColumnName("ExtendedProperties")
                .HasMaxLength(EXTENDEDPROPERTIES_LENGTH);
        }
    }
}

