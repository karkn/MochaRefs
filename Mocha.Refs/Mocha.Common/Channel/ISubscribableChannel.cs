using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Channel
{
    public interface ISubscribableChannel: IChannel
    {
        void Subscribe<T, TConsumer>() where TConsumer : IMessageConsumer<T>;
    }
}
