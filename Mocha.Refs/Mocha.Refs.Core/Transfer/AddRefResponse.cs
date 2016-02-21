using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class AddRefResponse
    {
        public EntityIdentity RefListIdentity { get; set; }
        public long RefId { get; set; }
    }
}
