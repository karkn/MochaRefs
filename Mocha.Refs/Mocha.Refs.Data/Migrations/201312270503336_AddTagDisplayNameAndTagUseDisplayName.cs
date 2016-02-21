namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagDisplayNameAndTagUseDisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TagUses", "DisplayName", c => c.String());
            AddColumn("dbo.Tags", "DisplayName", c => c.String());
            
            Sql("UPDATE dbo.TagUses SET DisplayName = (SELECT Name FROM dbo.Tags WHERE dbo.TagUses.TagId = dbo.Tags.Id)");
            Sql("UPDATE dbo.Tags SET DisplayName = Name");
            Sql("UPDATE dbo.Tags SET Name = Lower(Name)");
        }
        
        public override void Down()
        {
            Sql("UPDATE dbo.Tags SET Name = (SELECT DisplayName FROM dbo.TagUses WHERE dbo.TagUses.TagId = dbo.Tags.Id)");

            DropColumn("dbo.Tags", "DisplayName");
            DropColumn("dbo.TagUses", "DisplayName");
        }
    }
}
