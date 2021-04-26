namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mergefix : DbMigration
    {
        public override void Up()
        {
         //   RenameColumn(table: "dbo.Containers", name: "UserEmail", newName: "UserID");
         //   RenameIndex(table: "dbo.Containers", name: "IX_UserEmail", newName: "IX_UserID");
          //  AddColumn("dbo.UserDomains", "Selected", c => c.Boolean(nullable: false));
          //  AddColumn("dbo.Events", "DomainID", c => c.Guid());
        }
        
        public override void Down()
        {
         //   DropColumn("dbo.Events", "DomainID");
         //   DropColumn("dbo.UserDomains", "Selected");
         //   RenameIndex(table: "dbo.Containers", name: "IX_UserID", newName: "IX_UserEmail");
         //   RenameColumn(table: "dbo.Containers", name: "UserID", newName: "UserEmail");
        }
    }
}
