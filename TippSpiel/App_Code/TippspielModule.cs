using Ninject.Modules;
using FussballTipp.Utils;
using Tippspiel.Contracts;

namespace TippSpiel.App_Code
{
    public class TippspielModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ICacheProvider>().To<DefaultCacheProvider>();
        }
    }
}