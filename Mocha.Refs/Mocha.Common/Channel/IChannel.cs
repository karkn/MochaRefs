using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Channel
{
    public interface IChannel
    {
        void Send<T>(T message);
    }
}
