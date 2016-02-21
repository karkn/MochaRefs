namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetTagStatisticsData : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO dbo.TagStatistics SELECT Id, 0 FROM dbo.Tags");
            Sql(@"
UPDATE dbo.TagStatistics
  SET RefListcount = (
    SELECT COUNT(*) FROM dbo.Tags, dbo.TagUses, dbo.RefListTagUses, dbo.RefLists
      WHERE dbo.TagStatistics.TagId = dbo.Tags.Id
        AND dbo.Tags.Id = dbo.TagUses.TagId
        AND dbo.TagUses.Id = dbo.RefListTagUses.TagUseId
        AND dbo.RefListTagUses.RefListId = dbo.RefLists.Id
        AND dbo.RefLists.PublishingStatus = 0
   )
"
            );

        }
        
        public override void Down()
        {
        }
    }
}
