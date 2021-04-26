namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class barcodevalidate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "barcodeValidate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "barcodeValidate");
        }
    }
}
