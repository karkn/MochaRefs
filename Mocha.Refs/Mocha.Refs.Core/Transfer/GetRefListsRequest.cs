using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Transfer
{
    [Serializable]
    public class GetRefListsRequest
    {
        public long? AuthorId { get; set; }
        public string AuthorUserName { get; set; }

        public long? TagUseId { get; set; }
        public string TagName { get; set; }

        public string TitleSearch { get; set; }

        public PublishingStatusConditionKind PublishingStatusCondition { get; set; }

        public DateTime? FromDate { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public RefListSortKind Sort { get; set; }

        public GetRefListsRequest(
            long? authorId, string authorUserName, long? tagUseId, string tagName,
            string titleSearch,
            DateTime? fromDate,
            PublishingStatusConditionKind publishingStatusCondition,
            int pageIndex, int pageSize,
            RefListSortKind sort
        )
        {
            AuthorId = authorId;
            AuthorUserName = authorUserName;

            TagUseId = tagUseId;
            TagName = tagName;

            TitleSearch = titleSearch;

            FromDate = fromDate;

            PublishingStatusCondition = publishingStatusCondition;

            PageIndex = pageIndex;
            PageSize = pageSize;

            Sort = sort;
        }
    }
}
