namespace Mocha.Refs.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeRefListTagUsesColumns : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RefListTagUses", name: "RefList_Id", newName: "RefListId");
            RenameColumn(table: "dbo.RefListTagUses", name: "TagUse_Id", newName: "TagUseId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.RefListTagUses", name: "TagUseId", newName: "TagUse_Id");
            RenameColumn(table: "dbo.RefListTagUses", name: "RefListId", newName: "RefList_Id");
        }
    }
}
