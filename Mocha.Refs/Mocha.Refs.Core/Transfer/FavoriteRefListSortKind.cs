using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    public enum FavoriteRefListSortKind : byte
    {
        RefListUpdatedDateDescending = 0,
        RefListUpdatedDateAscending = 1,
        RefListCreatedDateDescending = 2,
        RefListCreatedDateAscending = 3,
        RefListPublishedDateDescending = 6,
        FavoriteCountDescending = 4,
        FavoriteCreatedDescending = 5,
    }
}
