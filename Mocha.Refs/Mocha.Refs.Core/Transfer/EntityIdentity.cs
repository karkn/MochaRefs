using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class EntityIdentity
    {
        public long Id { get; set; }
        public byte[] RowVersion { get; set; }

        public EntityIdentity()
        {
        }

        public EntityIdentity(long id, byte[] rowVersion)
        {
            Id = id;
            RowVersion = rowVersion;
        }
    }
}
