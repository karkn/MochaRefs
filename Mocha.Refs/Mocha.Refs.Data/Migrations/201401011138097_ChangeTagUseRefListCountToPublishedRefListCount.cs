namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTagUseRefListCountToPublishedRefListCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TagUseStatistics", "PublishedRefListCount", c => c.Int(nullable: false));
            DropColumn("dbo.TagUseStatistics", "RefListCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TagUseStatistics", "RefListCount", c => c.Int(nullable: false));
            DropColumn("dbo.TagUseStatistics", "PublishedRefListCount");
        }
    }
}
