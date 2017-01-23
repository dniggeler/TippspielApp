using Ninject.Modules;
using Tippspiel.Contracts;
using Ninject.Web.Common;

namespace Tippspiel.Implementation
{
    public class ClientModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ICacheProvider>()
                .To<DefaultCacheProvider>()
                .InSingletonScope();

            Kernel.Bind<IMatchHistory>()
                .ToConstructor(c=>new OpenligaHistoryStorage(SportsdataConfigInfo.Current))
                .InRequestScope();
        }
    }
}