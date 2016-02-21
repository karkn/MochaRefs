namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFavoriteUserAndTag : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Favorites", "RefListId", "dbo.RefLists");
            DropIndex("dbo.Favorites", new[] { "RefListId" });
            AddColumn("dbo.Favorites", "Kind", c => c.Byte(nullable: false));
            AddColumn("dbo.Favorites", "TagId", c => c.Long());
            AddColumn("dbo.Favorites", "UserId", c => c.Long());
            AlterColumn("dbo.Favorites", "RefListId", c => c.Long());
            CreateIndex("dbo.Favorites", "TagId");
            CreateIndex("dbo.Favorites", "UserId");
            CreateIndex("dbo.Favorites", "RefListId");
            AddForeignKey("dbo.Favorites", "TagId", "dbo.Tags", "Id");
            AddForeignKey("dbo.Favorites", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.Favorites", "RefListId", "dbo.RefLists", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favorites", "RefListId", "dbo.RefLists");
            DropForeignKey("dbo.Favorites", "UserId", "dbo.Users");
            DropForeignKey("dbo.Favorites", "TagId", "dbo.Tags");
            DropIndex("dbo.Favorites", new[] { "RefListId" });
            DropIndex("dbo.Favorites", new[] { "UserId" });
            DropIndex("dbo.Favorites", new[] { "TagId" });
            AlterColumn("dbo.Favorites", "RefListId", c => c.Long(nullable: false));
            DropColumn("dbo.Favorites", "UserId");
            DropColumn("dbo.Favorites", "TagId");
            DropColumn("dbo.Favorites", "Kind");
            CreateIndex("dbo.Favorites", "RefListId");
            AddForeignKey("dbo.Favorites", "RefListId", "dbo.RefLists", "Id", cascadeDelete: true);
        }
    }
}
