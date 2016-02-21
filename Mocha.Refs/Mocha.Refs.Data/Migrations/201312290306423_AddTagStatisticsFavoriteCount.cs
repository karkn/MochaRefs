namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagStatisticsFavoriteCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TagStatistics", "FavoriteCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TagStatistics", "FavoriteCount");
        }
    }
}
