using CustomeSerilogExample.Repository.Interfaces;
using Serilog.Context;

namespace CustomeSerilogExample.Repository.Classes
{
    public class LogManagerServices : ILogServices
    {
        private readonly ILogger<LogManagerServices> _services;

        public LogManagerServices(ILogger<LogManagerServices> services)
        {
            _services = services;
        }
        public async Task LogManager(string Controller, string Method, string UserId, Int32 Flag, String Exception, String MethodType)
        {
            TimeZoneInfo currentZone = TimeZoneInfo.Local;
            // Get the current UTC time
            DateTime utcNow = DateTime.UtcNow;
            // Convert UTC time to local time
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, currentZone);

            //if (Flag == 1)
            //{
            //    LogContext.PushProperty("UserId", UserId);
            //    LogContext.PushProperty("ControllerName", Controller);
            //    LogContext.PushProperty("MethodName", Controller);
            //    LogContext.PushProperty("MethodType", MethodType);


            //    _services.LogWarning("Request made to {ControllerName}/{MethodName} by the {UserId} at {localTime}", Controller, Method, UserId, localTime);
            //}
            //else
            if (Flag == 2)
            {
                LogContext.PushProperty("ControllerName", Controller);
                LogContext.PushProperty("MethodName", Method);
                LogContext.PushProperty("MethodType", MethodType);
                LogContext.PushProperty("AccessDateTime", localTime);
                _services.LogError(Exception, "Error Found on {ControllerName}/{MethodName}", Controller, Method);
            }
        }
    }
}
