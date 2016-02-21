using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.DataTypes
{
    [Serializable]
    public enum RefKind : byte
    {
        Link = 0,
        Book = 1,

        Heading = 10,
        Text = 11,
    }

    [Serializable]
    public enum PublishingStatusKind : byte
    {
        Publish = 0,
        Private = 1,
        Draft = 2,
    }

    [Serializable]
    public enum FavoriteKind: byte
    {
        RefList = 0,
        User = 1,
        Tag = 2,
    }

    //[Serializable]
    //public enum RefListKind : byte
    //{
    //    Normal = 0,
    //    Unfiled = 1,
    //}
}
