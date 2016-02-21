namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetStatisticsCascadeDeleteTrue : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RefListStatistics", "RefListId", "dbo.RefLists");
            DropForeignKey("dbo.TagUseStatistics", "TagUseId", "dbo.TagUses");
            DropForeignKey("dbo.TagStatistics", "TagId", "dbo.Tags");
            DropIndex("dbo.RefListStatistics", new[] { "RefListId" });
            DropIndex("dbo.TagUseStatistics", new[] { "TagUseId" });
            DropIndex("dbo.TagStatistics", new[] { "TagId" });
            CreateIndex("dbo.RefListStatistics", "RefListId");
            CreateIndex("dbo.TagUseStatistics", "TagUseId");
            CreateIndex("dbo.TagStatistics", "TagId");
            AddForeignKey("dbo.RefListStatistics", "RefListId", "dbo.RefLists", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TagUseStatistics", "TagUseId", "dbo.TagUses", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TagStatistics", "TagId", "dbo.Tags", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagStatistics", "TagId", "dbo.Tags");
            DropForeignKey("dbo.TagUseStatistics", "TagUseId", "dbo.TagUses");
            DropForeignKey("dbo.RefListStatistics", "RefListId", "dbo.RefLists");
            DropIndex("dbo.TagStatistics", new[] { "TagId" });
            DropIndex("dbo.TagUseStatistics", new[] { "TagUseId" });
            DropIndex("dbo.RefListStatistics", new[] { "RefListId" });
            CreateIndex("dbo.TagStatistics", "TagId");
            CreateIndex("dbo.TagUseStatistics", "TagUseId");
            CreateIndex("dbo.RefListStatistics", "RefListId");
            AddForeignKey("dbo.TagStatistics", "TagId", "dbo.Tags", "Id");
            AddForeignKey("dbo.TagUseStatistics", "TagUseId", "dbo.TagUses", "Id");
            AddForeignKey("dbo.RefListStatistics", "RefListId", "dbo.RefLists", "Id");
        }
    }
}
