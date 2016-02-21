namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeRefListStaticsRequiredPrincipal : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RefListStatistics", "RefListId", "dbo.RefLists");
            DropIndex("dbo.RefListStatistics", new[] { "RefListId" });
            AlterColumn("dbo.RefLists", "Id", c => c.Long(nullable: false));
            CreateIndex("dbo.RefLists", "Id");
            AddForeignKey("dbo.RefLists", "Id", "dbo.RefListStatistics", "RefListId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RefLists", "Id", "dbo.RefListStatistics");
            DropIndex("dbo.RefLists", new[] { "Id" });
            AlterColumn("dbo.RefLists", "Id", c => c.Long(nullable: false, identity: true));
            CreateIndex("dbo.RefListStatistics", "RefListId");
            AddForeignKey("dbo.RefListStatistics", "RefListId", "dbo.RefLists", "Id");
        }
    }
}
