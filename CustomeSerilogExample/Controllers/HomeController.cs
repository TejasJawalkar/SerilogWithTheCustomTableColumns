using System.Diagnostics;
using CustomeSerilogExample.Models;
using CustomeSerilogExample.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomeSerilogExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogServices _logServices;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(ILogServices logServices, IHttpContextAccessor contextAccessor)
        {
            _logServices = logServices;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            try
            {
                var userId = "12121"; // Example: Get UserId from request header or context

                var data = int.Parse(userId);
                // Use LogContext to enrich the log dynamically

                _logServices.LogManager(_contextAccessor.HttpContext?.Request?.RouteValues["Controller"]?.ToString(), _contextAccessor.HttpContext?.Request?.RouteValues["action"]?.ToString(), userId);
                return View();

            }

            catch (Exception ex)
            {

                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
