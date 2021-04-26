namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarcodeUniqueness2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Items", "Barcode", unique: true, name: "Ix_Barcode");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Items", "Ix_Barcode");
        }
    }
}
