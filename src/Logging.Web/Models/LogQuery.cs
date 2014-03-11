using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Takenet.Library.Logging.Web.Models
{
    public class LogQuery
    {
        public LogQuery()
        {
            Applications = new string[0];
            Severity = TraceEventType.Information;
            Interval = LogQueryInterval.LastHour;

            Skip = 0;
            Take = 10;
        }

        public LogQueryInterval Interval { get; set; }

        public DateTime? CustomBeginDate { get; set; }

        public DateTime? CustomEndDate { get; set; }

        public string[] Applications { get; set; }

        public TraceEventType Severity { get; set; }
        
        public string Message { get; set; }

        [Display(Name = "Machine")]
        public string MachineName { get; set; }

        public long? CorrelationId { get; set; }

        public string Category { get; set; }

        public int Skip { get; set; }

        [Display(Name = "Page size")]
        public int Take { get; set; }
    }

    public enum LogQueryInterval
    {
        LastMinute,
        LastHour,
        LastDay,
        LastTwoDays,
        LastWeek,
        Custom
    }
}