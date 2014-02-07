using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Takenet.Library.Data;
using Takenet.Library.Data.EntityFramework;
using Takenet.Library.Logging;
using Takenet.Library.Logging.EntityFramework.Configurations;

namespace Takenet.Library.Logging.EntityFramework.Repositories
{
    public class LogMessageRepository : EntityRepository<LogMessage, long>
    {
        public LogMessageRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork as DbContext)
        {
        }

        public override void Add(LogMessage entity, bool isNew)
        {
            entity.ApplicationName = entity.ApplicationName.LeftTruncate(LogMessageConfiguration.APPLICATIONNAME_LENGTH);
            entity.MachineName = entity.MachineName.LeftTruncate(LogMessageConfiguration.MACHINENAME_LENGTH);
            entity.ProcessName = entity.ProcessName.LeftTruncate(LogMessageConfiguration.PROCESSNAME_LENGTH);
            entity.UserName = entity.UserName.LeftTruncate(LogMessageConfiguration.USERNAME_LENGTH);
            entity.Message = entity.Message.LeftTruncate(LogMessageConfiguration.MESSAGE_LENGTH);
            entity.Title = entity.Title.LeftTruncate(LogMessageConfiguration.TITLE_LENGTH);
            entity.ExtendedPropertiesFlat = entity.ExtendedPropertiesFlat.LeftTruncate(LogMessageConfiguration.EXTENDEDPROPERTIES_LENGTH);
            entity.CategoriesFlat = entity.CategoriesFlat.LeftTruncate(LogMessageConfiguration.CATEGORIES_LENGTH);

            base.Add(entity, isNew);
        }
    }

    public static class StringExtensions
    {
        public static string LeftTruncate(this string value, int length)
        {
            if (value != null &&
                value.Length > length)
            {
                return value.Substring(0, length);
            }
            else
            {
                return value;
            }
        }
    }
}
