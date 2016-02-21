using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Unity
{
    public static class MochaContainer
    {
        public static Action<IUnityContainer> RegisterTypes = null;

        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            if (RegisterTypes == null)
            {
                throw new InvalidOperationException("RegisterTypes must be set");
            }
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer GetContainer()
        {
            return container.Value;
        }

        public static T Resolve<T>()
        {
            return GetContainer().Resolve<T>();
        }

        public static T Resolve<T>(string name)
        {
            return GetContainer().Resolve<T>(name);
        }
    }
}
