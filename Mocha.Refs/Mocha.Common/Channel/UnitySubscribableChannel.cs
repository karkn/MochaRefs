using Microsoft.Practices.Unity;
using Mocha.Common.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Channel
{
    /// <summary>
    /// 同期的に送信する。
    /// </summary>
    public class UnitySubscribableChannel: ISubscribableChannel
    {
        public void Subscribe<T, TConsumer>() where TConsumer : IMessageConsumer<T>
        {
            var container = MochaContainer.GetContainer();

            var consumerInterfaceType = typeof(IMessageConsumer<T>);
            var name = typeof(TConsumer).FullName;

            container.RegisterType<IMessageConsumer<T>, TConsumer>(name);
        }

        public void Send<T>(T message)
        {
            var container = MochaContainer.GetContainer();

            var consumerInterfaceType = typeof(IMessageConsumer<T>);
            var resolved = container.ResolveAll(consumerInterfaceType);
            var consumers = resolved.Cast<IMessageConsumer<T>>();
            foreach (var consumer in consumers)
            {
                consumer.Consume(message);
            }
        }
    }
}
