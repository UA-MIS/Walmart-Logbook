namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newbarcodeuniqueness : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Items", "Ix_Barcode");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Items", "Barcode", unique: true, name: "Ix_Barcode");
        }
    }
}
