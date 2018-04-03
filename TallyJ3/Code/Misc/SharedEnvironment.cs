using Microsoft.Extensions.DependencyInjection;
using TallyJ3.Code.Helper;
using TallyJ3.Code.Hubs;

namespace TallyJ3.Code.Misc
{
    public static class SharedEnvironment
    {
        public static ILogHelper LogHelper
        {
            get
            {
                return Startup.ServiceProvider.GetService<ILogHelper>();
            }
        }
        public static IPublicHub PublicHub
        {
            get
            {
                return Startup.ServiceProvider.GetService<IPublicHub>();
            }
        }
        public static IMainHub MainHub
        {
            get
            {
                return Startup.ServiceProvider.GetService<IMainHub>();
            }
        }
        public static IAnalyzeHub AnalyzeHub
        {
            get
            {
                return Startup.ServiceProvider.GetService<IAnalyzeHub>();
            }
        }
    }
}
