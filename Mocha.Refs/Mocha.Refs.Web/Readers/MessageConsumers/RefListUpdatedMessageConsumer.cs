using Mocha.Common.Cache;
using Mocha.Common.Channel;
using Mocha.Refs.Core.Handlers.Messages;
using Mocha.Refs.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Readers.MessageConsumers
{
    public class RefListUpdatedMessageConsumer: IMessageConsumer<RefListUpdatedMessage>
    {
        IObjectCache<RefListViewModel> _refListCache;
        IObjectCache<ICollection<RefViewModel>> _refsCache;

        public RefListUpdatedMessageConsumer(
            IObjectCache<RefListViewModel> refListCache,
            IObjectCache<ICollection<RefViewModel>> refsCache
        )
        {
            _refListCache = refListCache;
            _refsCache = refsCache;
        }

        public void Consume(RefListUpdatedMessage message)
        {
            _refListCache.Remove(message.Id.ToString());
            _refsCache.Remove(message.Id.ToString());
        }
    }
}
