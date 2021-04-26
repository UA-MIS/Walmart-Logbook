namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Containers", "LocationID", c => c.Guid(nullable: false));
            AddColumn("dbo.Events", "ItemID", c => c.Guid(nullable: false));
            AddColumn("dbo.Items", "ContainerID", c => c.Guid(nullable: false));
            CreateIndex("dbo.Containers", "LocationID");
            CreateIndex("dbo.Events", "ItemID");
            CreateIndex("dbo.Items", "ContainerID");
            AddForeignKey("dbo.Containers", "LocationID", "dbo.Locations", "LocationID", cascadeDelete: true);
            AddForeignKey("dbo.Items", "ContainerID", "dbo.Containers", "ContainerID", cascadeDelete: true);
            AddForeignKey("dbo.Events", "ItemID", "dbo.Items", "ItemID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "ItemID", "dbo.Items");
            DropForeignKey("dbo.Items", "ContainerID", "dbo.Containers");
            DropForeignKey("dbo.Containers", "LocationID", "dbo.Locations");
            DropIndex("dbo.Items", new[] { "ContainerID" });
            DropIndex("dbo.Events", new[] { "ItemID" });
            DropIndex("dbo.Containers", new[] { "LocationID" });
            DropColumn("dbo.Items", "ContainerID");
            DropColumn("dbo.Events", "ItemID");
            DropColumn("dbo.Containers", "LocationID");
        }
    }
}
