using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Takenet.Library.Data;
using Takenet.Library.Logging.EntityFramework;
using Takenet.Library.Logging.EntityFramework.Repositories;
using Takenet.Library.Logging.Web.Models;

namespace Takenet.Library.Logging.Web.Controllers
{
    public class HomeController : Controller
    {
        private ILoggingContext _context;

        public HomeController()
        {
            _context = new LoggingContext();
        }

        public ActionResult Index()
        {
            return Index(
                new LogQuery()
                );
        }

        [HttpPost]
        public ActionResult Index(LogQuery logQuery)
        {
            ViewBag.Applications = _context
                .ApplicationConfigurationRepository
                .AsQueryable()
                .OrderBy(a => a.ApplicationName)
                .Select(a => new SelectListItem()
                {
                    Text = a.ApplicationName,
                    Value = a.ApplicationName,
                    Selected = logQuery.Applications.Contains(a.ApplicationName)
                });

            ViewBag.Severities = Enum.GetNames(typeof(TraceEventType));

            DateTime logBeginDate;

            switch (logQuery.Interval)
            {
                case LogQueryInterval.LastMinute:
                    logBeginDate = DateTime.Now.AddMinutes(-1);
                    break;
                case LogQueryInterval.LastHour:
                    logBeginDate = DateTime.Now.AddHours(-1);
                    break;
                case LogQueryInterval.LastDay:
                    logBeginDate = DateTime.Now.AddDays(-1);
                    break;
                case LogQueryInterval.LastTwoDays:
                    logBeginDate = DateTime.Now.AddDays(-2);
                    break;
                case LogQueryInterval.LastWeek:
                    logBeginDate = DateTime.Now.AddDays(-7);
                    break;
                case LogQueryInterval.Custom:
                    logBeginDate = logQuery.CustomBeginDate.Value;
                    break;
                default:
                    logBeginDate = DateTime.Now.AddSeconds(-10);
                    break;
            }

            var severityFlat = (int)logQuery.Severity;

            var logMessages = _context
                .LogMessageRepository
                .AsQueryable()
                .Where(l => l.Timestamp >= logBeginDate)
                .Where(l => logQuery.Applications.Contains(l.ApplicationName))
                .Where(l => l.SeverityFlat <= severityFlat);

            if (!string.IsNullOrWhiteSpace(logQuery.Message))
            {
                logMessages = logMessages
                    .Where(l => l.Message.Contains(logQuery.Message));
            }

            logMessages = logMessages
                .OrderByDescending(l => l.Timestamp)
                .Skip(logQuery.Skip)
                .Take(logQuery.Take);


            ViewBag.LogQuery = logQuery;

            return View(logMessages);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}