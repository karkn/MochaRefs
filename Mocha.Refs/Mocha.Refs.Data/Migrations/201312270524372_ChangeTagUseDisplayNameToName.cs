namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTagUseDisplayNameToName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TagUses", "Name", c => c.String());
            Sql("UPDATE dbo.TagUses SET Name = DisplayName");
            DropColumn("dbo.TagUses", "DisplayName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TagUses", "DisplayName", c => c.String());
            Sql("UPDATE dbo.TagUses SET DisplayName = Name");
            DropColumn("dbo.TagUses", "Name");
        }
    }
}
