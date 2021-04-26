namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class strLens2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Containers", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Domains", "DomainName", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Locations", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Locations", "Description", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Locations", "Description", c => c.String());
            AlterColumn("dbo.Locations", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Domains", "DomainName", c => c.String(nullable: false));
            AlterColumn("dbo.Containers", "Name", c => c.String(nullable: false));
        }
    }
}
