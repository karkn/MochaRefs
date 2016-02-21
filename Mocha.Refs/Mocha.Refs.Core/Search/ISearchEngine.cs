using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Search
{
    public interface ISearchEngine
    {
        void BuildIndex();
        IEnumerable<long> SearchRefList(string searchText, int count);
    }
}
