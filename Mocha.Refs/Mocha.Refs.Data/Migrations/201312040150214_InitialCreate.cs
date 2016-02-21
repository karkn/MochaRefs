namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MochaUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefLists",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        Comment = c.String(),
                        PublishingStatus = c.Byte(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        AuthorId = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        CreatedUserId = c.Long(nullable: false),
                        UpdatedUserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        MochaUserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TagUses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        TagId = c.Long(nullable: false),
                        OwnerId = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        CreatedUserId = c.Long(nullable: false),
                        UpdatedUserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.OwnerId)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.OwnerId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        CreatedUserId = c.Long(nullable: false),
                        UpdatedUserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Refs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Kind = c.Byte(nullable: false),
                        Uri = c.String(),
                        Title = c.String(),
                        Comment = c.String(),
                        IsRecommended = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        RefListId = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                        CreatedUserId = c.Long(nullable: false),
                        UpdatedUserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RefLists", t => t.RefListId, cascadeDelete: true)
                .Index(t => t.RefListId);
            
            CreateTable(
                "dbo.RefListStatistics",
                c => new
                    {
                        RefListId = c.Long(nullable: false),
                        ViewCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RefListId)
                .ForeignKey("dbo.RefLists", t => t.RefListId)
                .Index(t => t.RefListId);
            
            CreateTable(
                "dbo.RefListTagUses",
                c => new
                    {
                        RefList_Id = c.Long(nullable: false),
                        TagUse_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.RefList_Id, t.TagUse_Id })
                .ForeignKey("dbo.RefLists", t => t.RefList_Id, cascadeDelete: true)
                .ForeignKey("dbo.TagUses", t => t.TagUse_Id, cascadeDelete: true)
                .Index(t => t.RefList_Id)
                .Index(t => t.TagUse_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RefListTagUses", "TagUse_Id", "dbo.TagUses");
            DropForeignKey("dbo.RefListTagUses", "RefList_Id", "dbo.RefLists");
            DropForeignKey("dbo.RefListStatistics", "RefListId", "dbo.RefLists");
            DropForeignKey("dbo.Refs", "RefListId", "dbo.RefLists");
            DropForeignKey("dbo.RefLists", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.TagUses", "TagId", "dbo.Tags");
            DropForeignKey("dbo.TagUses", "OwnerId", "dbo.Users");
            DropIndex("dbo.RefListTagUses", new[] { "TagUse_Id" });
            DropIndex("dbo.RefListTagUses", new[] { "RefList_Id" });
            DropIndex("dbo.RefListStatistics", new[] { "RefListId" });
            DropIndex("dbo.Refs", new[] { "RefListId" });
            DropIndex("dbo.RefLists", new[] { "AuthorId" });
            DropIndex("dbo.TagUses", new[] { "TagId" });
            DropIndex("dbo.TagUses", new[] { "OwnerId" });
            DropTable("dbo.RefListTagUses");
            DropTable("dbo.RefListStatistics");
            DropTable("dbo.Refs");
            DropTable("dbo.Tags");
            DropTable("dbo.TagUses");
            DropTable("dbo.Users");
            DropTable("dbo.RefLists");
            DropTable("dbo.MochaUsers");
        }
    }
}
