namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarcodeDataTypeChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Barcode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "Barcode");
        }
    }
}
