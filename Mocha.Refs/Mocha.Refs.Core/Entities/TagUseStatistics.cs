using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class TagUseStatistics
    {
        public int RefListCount { get; set; }
        public int PublishedRefListCount { get; set; }

        public long TagUseId { get; set; }

        public virtual TagUse TagUse { get; set; }
    }
}
