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
        public async Task LogManager(string Controller, string Method, string UserId, Int32 Flag, String Exception)
        {
            if (Flag == 1)
            {
                LogContext.PushProperty("UserId", UserId);
                LogContext.PushProperty("ControllerName", Controller);
                LogContext.PushProperty("MethodName", Controller);

                _services.LogWarning("Request made to {ControllerName}/{MethodName}", Controller, Method);
            }
            else if (Flag == 2)
            {
                LogContext.PushProperty("UserId", UserId);
                LogContext.PushProperty("ControllerName", Controller);
                LogContext.PushProperty("MethodName", Controller);

                _services.LogError(Exception, "Error Found on {ControllerName}/{MethodName}", Controller, Method);
            }
        }
    }
}
