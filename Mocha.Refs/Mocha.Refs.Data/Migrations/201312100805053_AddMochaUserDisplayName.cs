namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMochaUserDisplayName : DbMigration
    {
        public override void Up()
        {
            //Sql("DROP VIEW dbo.MochaUsers");
            //Sql("CREATE VIEW [dbo].[MochaUsers] AS SELECT CAST(Id AS UNIQUEIDENTIFIER) AS Id, UserName, Email, DisplayName FROM Auth.dbo.Users");
            //AddColumn("dbo.MochaUsers", "DisplayName", c => c.String());
        }
        
        public override void Down()
        {
            //Sql("DROP VIEW dbo.MochaUsers");
            //Sql("CREATE VIEW [dbo].[MochaUsers] AS SELECT CAST(Id AS UNIQUEIDENTIFIER) AS Id, UserName, Email FROM Auth.dbo.Users");
            //DropColumn("dbo.MochaUsers", "DisplayName");
        }
    }
}
