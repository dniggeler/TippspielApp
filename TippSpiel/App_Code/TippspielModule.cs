﻿using log4net;
using Ninject.Modules;
using Ninject.Web.Common;
using Tippspiel.Contracts;
using Tippspiel.Helpers;
using Tippspiel.Implementation;

namespace BhFS.Tippspiel.Utils
{
    public class TippspielModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ICacheProvider>()
                .To<DefaultCacheProvider>()
                .InSingletonScope();

            Kernel.Bind<IFussballDataRepository>()
                .ToConstructor(c => new FussballTipp.Repository.BuLiDataRepository(SportsdataConfigInfo.Current,c.Inject<ICacheProvider>()))
                .InRequestScope();

            Kernel.Bind<ILog>()
                .ToMethod(c => LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType))
                .InRequestScope();
        }
    }
}