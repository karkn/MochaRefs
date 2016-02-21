using Microsoft.Practices.Unity;
using Mocha.Common.Unity;
using Mocha.Refs.Core.Repositories;
using Mocha.Refs.Core.Search;
using Mocha.Refs.Data;
using Mocha.Refs.Integration.Lucene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Bootstrapper
{
    public static class Bootstrapper
    {
        public static void InitDatabase()
        {
            RefsDbInitializer.Init();
        }

        public static void InitContainer(Func<LifetimeManager> perRequestLifetimeManagerProvider)
        {
            var container = MochaContainer.GetContainer();
            container.RegisterType<IRefsContext, RefsContext>(perRequestLifetimeManagerProvider());
            container.RegisterType<ISearchEngine, SearchEngine>(perRequestLifetimeManagerProvider());

            container.RegisterType<IRefsContext, RefsContext>("job");
            container.RegisterType<ISearchEngine, SearchEngine>("job");
        }
    }
}
