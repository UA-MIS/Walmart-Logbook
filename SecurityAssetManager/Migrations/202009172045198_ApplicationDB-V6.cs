namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationDBV6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "DateTimeCreated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "DateTimeCreated");
        }
    }
}
