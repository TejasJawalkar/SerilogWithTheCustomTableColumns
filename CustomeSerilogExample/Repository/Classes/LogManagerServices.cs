using CustomeSerilogExample.Repository.Interfaces;
using Serilog.Context;

namespace CustomeSerilogExample.Repository.Classes
{
    public class LogManagerServices : ILogServices
    {
        private readonly ILogger<LogManagerServices> _logger;

        public LogManagerServices(ILogger<LogManagerServices> logger)
        {
            _logger = logger;
        }

        public async Task LogManager(string controller, string method, string userId, int flag, string exception, string methodType, string Namespaces)
        {
            try
            {
                var localTime = GetLocalTime();

                // Push common properties to Serilog context
                using (LogContext.PushProperty("UserId", userId))
                using (LogContext.PushProperty("ControllerName", controller))
                using (LogContext.PushProperty("MethodName", method))
                using (LogContext.PushProperty("MethodType", methodType))
                using (LogContext.PushProperty("AccessDateTime", localTime))
                using (LogContext.PushProperty("Namespaces", Namespaces))
                {
                    if (flag == 1)
                    {
                        _logger.LogWarning("{UserId} logged in at {AccessDateTime}", userId, localTime);
                    }
                    else if (flag == 2)
                    {
                        // Log all error details with contextual information
                        _logger.LogError(
                            "{Exception}. Details: NameSpace= {Namespaces}  Controller={ControllerName}, Method={MethodName}, UserId={UserId}, MethodType={MethodType}, AccessDateTime={AccessDateTime}",
                           Namespaces, exception, controller, method, userId, methodType, localTime
                        );
                    }
                    else
                    {
                        _logger.LogInformation("Unrecognized flag: {Flag}. Controller: {ControllerName}, Method: {MethodName}", flag, controller, method);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any issues during logging and include controller/method details
                _logger.LogError(
                    ex,
                    "An error occurred in LogManager for Controller={ControllerName}, Method={MethodName}. Additional details: UserId={UserId}, MethodType={MethodType}, AccessDateTime={AccessDateTime}",
                    controller, method, userId, methodType, GetLocalTime()
                );
            }

            // Simulate async work
            await Task.CompletedTask;
        }

        private DateTime GetLocalTime()
        {
            var currentZone = TimeZoneInfo.Local;
            var utcNow = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, currentZone);
        }
    }
}
