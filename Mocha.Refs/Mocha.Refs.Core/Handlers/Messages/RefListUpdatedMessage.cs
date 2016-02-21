using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers.Messages
{
    public class RefListUpdatedMessage
    {
        public long Id { get; private set; }

        public RefListUpdatedMessage(long id)
        {
            Id = id;
        }
    }
}
