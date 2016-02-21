using Mocha.Refs.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class SearchRefListsResponse
    {
        public int WholeCount { get; set; }
        public IEnumerable<RefList> RefLists { get; set; }
    }
}
