namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class revertingcheckboxes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Items", "barcodeValidate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "barcodeValidate", c => c.Boolean(nullable: false));
        }
    }
}
