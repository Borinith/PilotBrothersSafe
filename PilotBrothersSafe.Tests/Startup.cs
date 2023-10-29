using Microsoft.Extensions.DependencyInjection;
using PilotBrothersSafe.SafeLogic;

namespace PilotBrothersSafe.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ISafeLogic, SafeLogic.SafeLogic>();
        }
    }
}