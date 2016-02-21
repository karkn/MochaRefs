using Mocha.Common.Channel;
using Mocha.Common.Unity;
using Mocha.Refs.Core.Handlers.Messages;
using Mocha.Refs.Web.Readers.MessageConsumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web
{
    public static class ChannelConfig
    {
        public static void Config()
        {
            var channel = MochaContainer.Resolve<ISubscribableChannel>();
            channel.Subscribe<RefListUpdatedMessage, RefListUpdatedMessageConsumer>();
        }
    }
}