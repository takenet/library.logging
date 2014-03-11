using Mvc.JQuery.Datatables;
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
            ViewBag.Applications = _context
                .ApplicationConfigurationRepository
                .AsQueryable()
                .OrderBy(a => a.ApplicationName)
                .Select(a => new SelectListItem()
                {
                    Text = a.ApplicationName,
                    Value = a.ApplicationName,
                    Selected = false
                });

            ViewBag.LogQuery = new LogQuery();


            return View(new LogQuery());
        }


        [HttpPost]
        public JsonResult GetLogs(LogQuery logQuery)
        {
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

            if (!string.IsNullOrWhiteSpace(logQuery.MachineName))
            {
                logMessages = logMessages
                    .Where(l => l.MachineName == logQuery.MachineName);
            }

            if (!string.IsNullOrWhiteSpace(logQuery.Category))
            {
                logMessages = logMessages
                    .Where(l => l.CategoriesFlat.Contains(logQuery.Category));
            }

            if (logQuery.CorrelationId.HasValue)
            {
                logMessages = logMessages
                    .Where(l => l.CorrelationId == logQuery.CorrelationId.Value);
            }

            var result = logMessages
                .OrderByDescending(l => l.Timestamp)
                .Skip(logQuery.Skip)
                .Take(logQuery.Take)
                .ToList()
                .Select(l => new
                {
                    LogMessageId = l.LogMessageSafeId,
                    Timestamp = l.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    l.Title,
                    l.ApplicationName,
                    Severity = l.Severity.ToString(),
                    l.MachineName,
                    l.Message,
                    l.UserName,
                    l.CorrelationId,
                    Categories = l.CategoriesFlat,
                    ExtendedProperties = l.ExtendedPropertiesFlat
                });

            return Json(result);
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