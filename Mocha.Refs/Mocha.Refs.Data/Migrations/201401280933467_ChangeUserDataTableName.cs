namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeUserDataTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UserDatas", newName: "UserData");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.UserData", newName: "UserDatas");
        }
    }
}
