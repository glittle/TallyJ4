using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using TallyJ3.Code.Helper;
using TallyJ3.Code.Hubs;

namespace TallyJ3.Code.Misc
{
    public class SharedEnvironment : ISharedEnvironment
    {
        private readonly IHubContext<PublicHubCore> _publicHubContext;

        public static ISharedEnvironment Current
        {
            get
            {
                return Startup.ServiceProvider.GetService<ISharedEnvironment>();
            }
        }

        public SharedEnvironment(ILogHelper logHelper, IHubContext<PublicHubCore> publicHubContext)
        {
            LogHelper = logHelper;
            _publicHubContext = publicHubContext;
        }

        //public ILogHelper LogHelper
        //{
        //    get
        //    {
        //        return Startup.ServiceProvider.GetService<ILogHelper>();
        //    }
        //}

        //public IPublicHub PublicHub
        //{
        //    get
        //    {
        //        return Startup.ServiceProvider.GetService<IPublicHub>();
        //    }
        //}
        //public IMainHub MainHub
        //{
        //    get
        //    {
        //        return Startup.ServiceProvider.GetService<IMainHub>();
        //    }
        //}
        //public IAnalyzeHub AnalyzeHub
        //{
        //    get
        //    {
        //        return Startup.ServiceProvider.GetService<IAnalyzeHub>();
        //    }
        //}

        public ILogHelper LogHelper { get; }
        public PublicHubHelper PublicHubHelper
        {
            get
            {
                return new PublicHubHelper(_publicHubContext);
            }
        }

        //public IPublicHub PublicHub { get; }
        public IMainHub MainHub { get; }
        public IAnalyzeHub AnalyzeHub { get; }
    }

    public interface ISharedEnvironment
    {
        PublicHubHelper PublicHubHelper { get; }

        ILogHelper LogHelper { get; }
        //IPublicHub PublicHub { get; }
        IMainHub MainHub { get; }
        IAnalyzeHub AnalyzeHub { get; }
    }
}
