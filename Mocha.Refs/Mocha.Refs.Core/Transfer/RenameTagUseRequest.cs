using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class RenameTagUseRequest
    {
        public EntityIdentity TagUseIdentity { get; set; }
        public long OwnerId { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
    }
}
