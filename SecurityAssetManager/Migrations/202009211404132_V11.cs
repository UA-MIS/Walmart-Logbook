namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V11 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "ItemID", "dbo.Items");
            DropIndex("dbo.Events", new[] { "ItemID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Events", "ItemID");
            AddForeignKey("dbo.Events", "ItemID", "dbo.Items", "ItemID", cascadeDelete: true);
        }
    }
}
