using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mocha.Refs.Web.Models
{
    [Serializable]
    public class RefListViewModel
    {
        public long Id { get; set; }

        public UserViewModel Author { get; set; }

        public RefListStatisticsViewModel Statistics { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public PublishingStatusKind PublishingStatus { get; set; }

        public DateTime? PublishedDate { get; set; }

        public IEnumerable<string> TagUses { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
