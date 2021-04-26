namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Containers", "isActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Locations", "isActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Items", "needWitness", c => c.Boolean(nullable: false));
            AddColumn("dbo.Items", "isActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "isActive");
            DropColumn("dbo.Items", "needWitness");
            DropColumn("dbo.Locations", "isActive");
            DropColumn("dbo.Containers", "isActive");
        }
    }
}
