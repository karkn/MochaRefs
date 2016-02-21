namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFavriteUserToOwner : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Favorites", new[] { "UserId" });
            DropForeignKey("dbo.Favorites", "UserId", "dbo.Users");
            RenameColumn(table: "dbo.Favorites", name: "UserId", newName: "OwnerId");
            CreateIndex("dbo.Favorites", "OwnerId");
            AddForeignKey("dbo.Favorites", "OwnerId", "dbo.Users", "Id");

        }
        
        public override void Down()
        {
            DropIndex("dbo.Favorites", new[] { "OwnerId" });
            DropForeignKey("dbo.Favorites", "OwnerId", "dbo.Users");
            RenameColumn(table: "dbo.Favorites", name: "OwnerId", newName: "UserId");
            CreateIndex("dbo.Favorites", "UserId");
            AddForeignKey("dbo.Favorites", "UserId", "dbo.Users", "Id");
        }
    }
}
