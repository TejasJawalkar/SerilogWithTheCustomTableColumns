using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace CustomeSerilogExample.Filters
{
    public class LoggerFilter : IActionFilter
    {
        private readonly ILogger<LoggerFilter> _logger;
        private readonly IHttpContextAccessor _httpContext;

        public LoggerFilter(ILogger<LoggerFilter> logger, IHttpContextAccessor httpContext)
        {
            _logger = logger;
            _httpContext = httpContext;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                var userId = "10";
                var controllerName = _httpContext.HttpContext?.Request.RouteValues["controller"]?.ToString();
                var actionName = _httpContext.HttpContext?.Request.RouteValues["action"]?.ToString();
                var methodType = _httpContext.HttpContext?.Request.Method;
                TimeZoneInfo currentZone = TimeZoneInfo.Local;
                // Get the current UTC time
                DateTime utcNow = DateTime.UtcNow;
                // Convert UTC time to local time
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, currentZone);

                // Push these properties to Serilog's context
                LogContext.PushProperty("UserId", userId);
                LogContext.PushProperty("ControllerName", controllerName);
                LogContext.PushProperty("MethodName", actionName);
                LogContext.PushProperty("MethodType", methodType);
                LogContext.PushProperty("AccessDateTime", localTime);

                // Log the information
                _logger.LogInformation(
                    "User {UserId} accessed {ControllerName}/{ActionName} using {MethodType} at {AccessTime}", userId, controllerName, actionName, methodType, localTime);

            }
            catch (Exception ex)
            {
                var controllerName = _httpContext.HttpContext?.Request.RouteValues["controller"]?.ToString();
                var actionName = _httpContext.HttpContext?.Request.RouteValues["action"]?.ToString();
                var methodType = _httpContext.HttpContext?.Request.Method;
                TimeZoneInfo currentZone = TimeZoneInfo.Local;
                DateTime utcNow = DateTime.UtcNow;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, currentZone);

                LogContext.PushProperty("ControllerName", controllerName);
                LogContext.PushProperty("ActionName", actionName);
                LogContext.PushProperty("MethodType", methodType);
                LogContext.PushProperty("AccessDateTime", localTime);
                _logger.LogError(ex.Message, "Error in {ControllerName}/{ActionName} using {MethodType} at {AccessTime}");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
