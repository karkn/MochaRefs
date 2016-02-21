namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserDisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "DisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DisplayName");
        }
    }
}
