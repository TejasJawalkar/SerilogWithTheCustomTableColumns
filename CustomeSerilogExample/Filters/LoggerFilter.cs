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
                // Fetch common information
                var userId = "10"; // Replace with actual user ID retrieval logic if available
                var controllerName = GetRouteValue("controller");
                var actionName = GetRouteValue("action");
                var methodType = _httpContext.HttpContext?.Request.Method;
                var namespaceName = this.GetType().Namespace ?? "UnknownNamespace";
                var localTime = GetLocalTime();

                // Push properties to Serilog's logging context
                using (LogContext.PushProperty("UserId", userId))
                using (LogContext.PushProperty("ControllerName", controllerName))
                using (LogContext.PushProperty("MethodName", actionName))
                using (LogContext.PushProperty("MethodType", methodType))
                using (LogContext.PushProperty("AccessDateTime", localTime))
                using (LogContext.PushProperty("Namespaces", namespaceName))
                {
                    // Log information
                    _logger.LogInformation(
                        "User {UserId} accessed {ControllerName}/{ActionName} using {MethodType} at {AccessDateTime}",
                        userId, controllerName, actionName, methodType, localTime);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Logic for pre-action execution logging (optional)
        }

        private string? GetRouteValue(string key)
        {
            return _httpContext.HttpContext?.Request.RouteValues[key]?.ToString();
        }

        private DateTime GetLocalTime()
        {
            var currentZone = TimeZoneInfo.Local;
            var utcNow = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, currentZone);
        }

        private void LogException(Exception ex)
        {
            try
            {
                var controllerName = GetRouteValue("controller");
                var actionName = GetRouteValue("action");
                var methodType = _httpContext.HttpContext?.Request.Method;
                var namespaceName = this.GetType().Namespace ?? "UnknownNamespace";
                var localTime = GetLocalTime();

                using (LogContext.PushProperty("ControllerName", controllerName))
                using (LogContext.PushProperty("ActionName", actionName))
                using (LogContext.PushProperty("MethodType", methodType))
                using (LogContext.PushProperty("AccessDateTime", localTime))
                using (LogContext.PushProperty("Namespaces", namespaceName))
                {
                    _logger.LogError(
                        ex,
                        "An error occurred in {ControllerName}/{ActionName} using {MethodType} at {AccessDateTime}",
                        controllerName, actionName, methodType, localTime);
                }
            }
            catch
            {
                // Fallback in case logging itself fails
                _logger.LogError("An exception occurred while logging an exception: {ExceptionMessage}", ex.Message);
            }
        }
    }
}
