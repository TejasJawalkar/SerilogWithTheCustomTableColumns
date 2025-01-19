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

        [HttpGet]
        public IActionResult Index()
        {
            var userId = "invalidUserId"; // Example: Get UserId from request header or context
            try
            {
                var data = int.Parse(userId);
                // Use LogContext to enrich the log dynamically

                _logServices.LogManager(_contextAccessor.HttpContext?.Request?.RouteValues["Controller"]?.ToString(), _contextAccessor.HttpContext?.Request?.RouteValues["action"]?.ToString(), userId, 1, "",
                  _contextAccessor.HttpContext.Request.Method.ToString());
                return View();
            }

            catch (Exception ex)
            {

                _logServices.LogManager(_contextAccessor.HttpContext?.Request?.RouteValues["Controller"]?.ToString(), _contextAccessor.HttpContext?.Request?.RouteValues["action"]?.ToString(), userId, 2, ex.Message,
                    _contextAccessor.HttpContext.Request.Method.ToString());
                return View();
            }
        }

        [HttpGet]
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
