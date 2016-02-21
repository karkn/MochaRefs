namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixRefListStatisticsLinkCount : DbMigration
    {
        public override void Up()
        {
            var query = @"
UPDATE [dbo].[RefListStatistics]
  SET LinkCount = (
    SELECT COUNT(*) FROM [dbo].[Refs]
    WHERE [dbo].[Refs].RefListId = [dbo].[RefListStatistics].RefListId
      AND Kind = 0
  );";
            Sql(query);
        }
        
        public override void Down()
        {
        }
    }
}
