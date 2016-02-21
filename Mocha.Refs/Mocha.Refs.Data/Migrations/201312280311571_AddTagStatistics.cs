namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagStatistics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagStatistics",
                c => new
                    {
                        TagId = c.Long(nullable: false),
                        RefListCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TagId)
                .ForeignKey("dbo.Tags", t => t.TagId)
                .Index(t => t.TagId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagStatistics", "TagId", "dbo.Tags");
            DropIndex("dbo.TagStatistics", new[] { "TagId" });
            DropTable("dbo.TagStatistics");
        }
    }
}
