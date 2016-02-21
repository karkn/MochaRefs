using Mocha.Refs.Core.DataTypes;
using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class CreateRefListRequest : EntityIdentity
    {
        public string Title { get; set; }
        public string Comment { get; set; }
        public PublishingStatusKind PublishingStatus { get; set; }
        public ICollection<string> TagUses { get; set; }
        public ICollection<Ref> Refs { get; set; }
    }
}
