namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefListStatisticsRefListCoutAndFavoriteCount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RefListStatistics", "FavoriteCount", c => c.Int(nullable: false));
            AddColumn("dbo.RefListStatistics", "LinkCount", c => c.Int(nullable: false));

            var query = @"
UPDATE [Refs].[dbo].[RefListStatistics]
  SET LinkCount = (
    SELECT COUNT(*) FROM [Refs].[dbo].[Refs]
    WHERE [Refs].[dbo].[Refs].RefListId = [Refs].[dbo].[RefListStatistics].RefListId
      AND Kind = 0
  );";
            Sql(query);
        }
        
        public override void Down()
        {
            DropColumn("dbo.RefListStatistics", "LinkCount");
            DropColumn("dbo.RefListStatistics", "FavoriteCount");
        }
    }
}
