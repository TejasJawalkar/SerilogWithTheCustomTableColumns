using CustomeSerilogExample.Repository.Classes;
using CustomeSerilogExample.Repository.Interfaces;

namespace CustomeSerilogExample.DependencyInjection
{
    public class DependencyInjector
    {
        public static void Injector(IServiceCollection services)
        {
            services.AddSingleton<ILogServices, LogManagerServices>();
        }
    }
}
