namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefListPublishedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RefLists", "PublishedDate", c => c.DateTime());
            Sql("UPDATE dbo.RefLists SET PublishedDate = CreatedDate");
        }
        
        public override void Down()
        {
            DropColumn("dbo.RefLists", "PublishedDate");
        }
    }
}
