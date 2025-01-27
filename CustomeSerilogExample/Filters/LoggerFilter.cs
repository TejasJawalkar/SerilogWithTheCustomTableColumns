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
                var UTCTime = GetLocalTime();

                // Push properties to Serilog's logging context
                using (LogContext.PushProperty("UserId", userId))
                using (LogContext.PushProperty("ControllerName", controllerName))
                using (LogContext.PushProperty("MethodName", actionName))
                using (LogContext.PushProperty("MethodType", methodType))
                using (LogContext.PushProperty("UTCTime", UTCTime))
                using (LogContext.PushProperty("Namespaces", namespaceName))
                {
                    // Log information
                    _logger.LogInformation(
                        "User {UserId} accessed {Namespaces}/{ControllerName}/{ActionName} using {MethodType} at {UTCTime}",
                        userId, namespaceName, controllerName, actionName, methodType, UTCTime);
                }
            }
            catch (Exception ex)
            {
                //This will Store the Error in DataBase
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
                var UTCTime = GetLocalTime();

                //This Will Push New Properties 
                using (LogContext.PushProperty("ControllerName", controllerName))
                using (LogContext.PushProperty("ActionName", actionName))
                using (LogContext.PushProperty("MethodType", methodType))
                using (LogContext.PushProperty("UTCTime", UTCTime))
                using (LogContext.PushProperty("Namespaces", namespaceName))
                {
                    _logger.LogError(
                        ex,
                        "An error occurred in {Namespaces}/{ControllerName}/{ActionName} using {MethodType} at {UTCTime}",
                        namespaceName, controllerName, actionName, methodType, UTCTime);
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
