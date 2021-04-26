namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class revert1433 : DbMigration
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
