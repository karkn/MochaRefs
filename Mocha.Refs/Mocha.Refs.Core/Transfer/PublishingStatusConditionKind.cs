using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    public enum PublishingStatusConditionKind: byte
    {
        All = 0,
        PublishOnly = 1,
        PrivateOnly = 2,
        DraftOnly = 3,
    }
}
