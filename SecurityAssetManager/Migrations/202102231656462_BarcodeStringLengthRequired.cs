namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarcodeStringLengthRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "Barcode", c => c.String(nullable: false, maxLength: 9));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Items", "Barcode", c => c.String());
        }
    }
}
