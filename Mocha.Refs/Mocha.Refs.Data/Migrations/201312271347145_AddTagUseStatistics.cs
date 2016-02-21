namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagUseStatistics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagUseStatistics",
                c => new
                    {
                        TagUseId = c.Long(nullable: false),
                        RefListCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TagUseId)
                .ForeignKey("dbo.TagUses", t => t.TagUseId)
                .Index(t => t.TagUseId);

            Sql("INSERT INTO dbo.TagUseStatistics SELECT Id AS TagUseId, 0 FROM dbo.TagUses");
            Sql(@"
UPDATE dbo.TagUseStatistics
  SET RefListcount = (
    SELECT COUNT(*) FROM dbo.TagUses, dbo.RefListTagUses
      WHERE dbo.TagUseStatistics.TagUseId = dbo.TagUses.Id
        AND dbo.TagUses.Id = dbo.RefListTagUses.TagUseId
  )"
            );
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagUseStatistics", "TagUseId", "dbo.TagUses");
            DropIndex("dbo.TagUseStatistics", new[] { "TagUseId" });
            DropTable("dbo.TagUseStatistics");
        }
    }
}
