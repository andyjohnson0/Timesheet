using Microsoft.AspNetCore.Mvc;
using Timesheet.App.Models;

namespace Timesheet.App.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<HomeController> _logger;


        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Add a timesheet entry
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult Add()
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
