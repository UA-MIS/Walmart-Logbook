namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeContainerFKUsertoUserID : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Containers", name: "UserEmail", newName: "UserID");
            RenameIndex(table: "dbo.Containers", name: "IX_UserEmail", newName: "IX_UserID");
        }
        
        public override void Down()
        {
         //   RenameIndex(table: "dbo.Containers", name: "IX_UserID", newName: "IX_UserEmail");
         //   RenameColumn(table: "dbo.Containers", name: "UserID", newName: "UserEmail");
        }
    }
}
