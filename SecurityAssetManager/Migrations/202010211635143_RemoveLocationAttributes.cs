namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLocationAttributes : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Locations", "StreetName");
            DropColumn("dbo.Locations", "StreetNum");
            DropColumn("dbo.Locations", "City");
            DropColumn("dbo.Locations", "State");
            DropColumn("dbo.Locations", "Country");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Locations", "Country", c => c.String(nullable: false));
            AddColumn("dbo.Locations", "State", c => c.String(nullable: false));
            AddColumn("dbo.Locations", "City", c => c.String(nullable: false));
            AddColumn("dbo.Locations", "StreetNum", c => c.String(nullable: false));
            AddColumn("dbo.Locations", "StreetName", c => c.String(nullable: false));
        }
    }
}
