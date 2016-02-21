using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using Mocha.Common.Unity;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Mocha.Refs.Web.App_Start.UnityWebActivator), "Start")]

namespace Mocha.Refs.Web.App_Start
{
    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() 
        {
            MochaContainer.RegisterTypes = UnityConfig.RegisterTypes;
            var container = MochaContainer.GetContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            // TODO: Uncomment if you want to use PerRequestLifetimeManager
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }
    }
}