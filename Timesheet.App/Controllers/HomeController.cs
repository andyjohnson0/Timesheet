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
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult Add(TimesheetEntry entry)
        {
            return Ok();
        }


        /// <summary>
        /// Get the timesheet as CSV text
        /// </summary>
        /// <returns>ActionResult encapsulating the CSV text</returns>
        [HttpGet]
        public ActionResult Timesheet()
        {
            return Ok("");
        }
    }
}
