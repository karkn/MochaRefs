namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTagDisplayName : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dbo.Tags SET Name = DisplayName");
            DropColumn("dbo.Tags", "DisplayName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tags", "DisplayName", c => c.String());
            Sql("UPDATE dbo.Tags SET DisplayName = Name");
            Sql("UPDATE dbo.Tags SET Name = Lower(Name)");
        }
    }
}
