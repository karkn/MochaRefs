using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Channel
{
    public interface IMessageConsumer<T>
    {
        void Consume(T message);
    }
}
