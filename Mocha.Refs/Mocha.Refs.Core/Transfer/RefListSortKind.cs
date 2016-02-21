using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    public enum RefListSortKind: byte
    {
        UpdatedDateDescending = 0,
        UpdatedDateAscending = 1,
        CreatedDateDescending = 2,
        CreatedDateAscending = 3,
        FavoriteCountDescending = 4,
        PublishedDateDescending = 5,
        ViewCountDescending = 6,
    }
}
