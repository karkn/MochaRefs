namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntegrateAuth : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        UserId = c.Long(nullable: false),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Users", "SecurityStamp", c => c.String());
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false));
            DropColumn("dbo.Users", "MochaUserId");
            //DropTable("dbo.MochaUsers");
            Sql("DROP VIEW dbo.MochaUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MochaUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(),
                        Email = c.String(),
                        DisplayName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Users", "MochaUserId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.UserLogins", "UserId", "dbo.Users");
            DropIndex("dbo.UserLogins", new[] { "UserId" });
            AlterColumn("dbo.Users", "UserName", c => c.String());
            DropColumn("dbo.Users", "SecurityStamp");
            DropTable("dbo.UserLogins");
        }
    }
}
