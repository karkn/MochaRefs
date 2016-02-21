using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Mocha.Refs.Core.Handlers;
using Mocha.Common.Cache;
using Mocha.Common.Channel;
using Mocha.Refs.Web.Models;
using System.Collections.Generic;
using Mocha.Refs.Web.Readers;
using Mocha.Refs.Web.Readers.MessageConsumers;
using Mocha.Refs.Core.Handlers.Messages;
using Mocha.Refs.Web.Filters;
using Mocha.Refs.Core;

namespace Mocha.Refs.Web.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ISystemHandler, SystemHandler>();
            container.RegisterType<IUserHandler, UserHandler>();
            container.RegisterType<ITagHandler, TagHandler>();
            container.RegisterType<IRefListHandler, RefListHandler>();
            container.RegisterType<IFavoriteHandler, FavoriteHandler>();

            container.RegisterType<IObjectCache<RefListViewModel>, ObjectCache<RefListViewModel>>(new InjectionConstructor());
            container.RegisterType<IObjectCache<ICollection<RefViewModel>>, ObjectCache<ICollection<RefViewModel>>>(new InjectionConstructor());
            container.RegisterType<RefListViewModelReader>();

            container.RegisterType<IUserContext, HttpContextUserContext>();

            container.RegisterType<IChannel, UnitySubscribableChannel>();
            container.RegisterType<ISubscribableChannel, UnitySubscribableChannel>();
        }
    }
}
