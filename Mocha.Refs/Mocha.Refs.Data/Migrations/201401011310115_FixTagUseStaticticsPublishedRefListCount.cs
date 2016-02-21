namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTagUseStaticticsPublishedRefListCount : DbMigration
    {
        public override void Up()
        {
            Sql(@"
UPDATE dbo.TagUseStatistics
  SET PublishedRefListcount = (
    SELECT COUNT(*) FROM dbo.TagUses, dbo.RefListTagUses, dbo.RefLists
      WHERE dbo.TagUseStatistics.TagUseId = dbo.TagUses.Id
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
