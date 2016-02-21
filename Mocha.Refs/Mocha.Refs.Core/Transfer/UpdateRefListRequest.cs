using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class UpdateRefListRequest : EntityIdentity
    {
        public string Title { get; set; }
        public string Comment { get; set; }
        public PublishingStatusKind PublishingStatus { get; set; }
        public ICollection<string> TagUses { get; set; }
    }
}
