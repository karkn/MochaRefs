namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagUseRefListCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TagUseStatistics", "RefListCount", c => c.Int(nullable: false));

            Sql(@"
UPDATE dbo.TagUseStatistics
  SET RefListcount = (
    SELECT COUNT(*) FROM dbo.TagUses, dbo.RefListTagUses
      WHERE dbo.TagUseStatistics.TagUseId = dbo.TagUses.Id
        AND dbo.TagUses.Id = dbo.RefListTagUses.TagUseId
  );
");
        }
        
        public override void Down()
        {
            DropColumn("dbo.TagUseStatistics", "RefListCount");
        }
    }
}
