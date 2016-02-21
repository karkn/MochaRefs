using Microsoft.Practices.Unity;
using Mocha.Common.Unity;
using Mocha.Refs.Core.Handlers;
using Mocha.Refs.Core.Repositories;
using Mocha.Refs.Core.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Handlers
{
    public class SystemHandler: ISystemHandler
    {
        public void BuildIndex()
        {
            var search = MochaContainer.Resolve<ISearchEngine>("job");
            search.BuildIndex();
        }

        public void UpdateTagStatistics()
        {
            /// jobから呼ばれるのでPerRequestライフサイクルな_refsContextは使えない
            using (var context = MochaContainer.Resolve<IRefsContext>("job"))
            {
                var query = @"
UPDATE dbo.TagStatistics
  SET RefListcount = (
    SELECT COUNT(*)
      FROM dbo.Tags, dbo.TagUses, dbo.RefListTagUses, dbo.RefLists
      WHERE dbo.TagStatistics.TagId = dbo.Tags.Id
        AND dbo.Tags.Id = dbo.TagUses.TagId
        AND dbo.TagUses.Id = dbo.RefListTagUses.TagUseId
        AND dbo.RefListTagUses.RefListId = dbo.RefLists.Id
        AND dbo.RefLists.PublishingStatus = 0
   )
";
                context.ExecuteSqlCommand(query);

                query = @"
UPDATE dbo.TagStatistics
  SET Favoritecount = (
    SELECT COUNT(*)
      FROM dbo.Tags AS tag, dbo.Favorites AS fav
      WHERE dbo.TagStatistics.TagId = tag.Id
        AND fav.Kind = 2
        AND fav.TagId = tag.Id
   )
";
                context.ExecuteSqlCommand(query);

            }
        }

    }
}
