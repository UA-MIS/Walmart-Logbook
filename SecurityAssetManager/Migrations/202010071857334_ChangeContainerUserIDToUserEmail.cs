namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeContainerUserIDToUserEmail : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Containers", name: "UserId", newName: "UserEmail");
            RenameIndex(table: "dbo.Containers", name: "IX_UserId", newName: "IX_UserEmail");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Containers", name: "IX_UserEmail", newName: "IX_UserId");
            RenameColumn(table: "dbo.Containers", name: "UserEmail", newName: "UserId");
        }
    }
}
