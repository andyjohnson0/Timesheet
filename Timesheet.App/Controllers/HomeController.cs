using Microsoft.AspNetCore.Mvc;
using Timesheet.App.Models;

namespace Timesheet.App.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(
            TimesheetDbContext db, 
            ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        private readonly TimesheetDbContext _db;
        private readonly ILogger<HomeController> _logger;


        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Add a timesheet entry
        /// </summary>
        /// <param name="entry">Timesheet entry</param>
        /// <returns>ActionResult encapsulating http status</returns>
        [HttpPost]
        public ActionResult Add(TimesheetEntry entry)
        {
            // Validation
            if (!ModelState.IsValid)
            {
                // Return with validation messages
                return View("Index", entry);
            }
            if (entry.Date > DateOnly.FromDateTime(DateTime.Now))
            {
                // Return with validation message
                ModelState?.AddModelError("Date", "Cannot submit future work items");
                return View("Index", entry);
            }

            var model = new TimesheetModel(_db);
            return model.AddEntry(entry) ? Ok() : Problem();
        }


        /// <summary>
        /// Get the timesheet as CSV text
        /// </summary>
        /// <returns>ActionResult encapsulating the CSV text</returns>
        [HttpGet]
        public ActionResult Timesheet()
        {
            var model = new TimesheetModel(_db);
            var csvText = model.GetTimesheetAsCSV();
            return Ok(csvText);
        }
    }
}
