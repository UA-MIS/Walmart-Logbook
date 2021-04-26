namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "ItemName");
        }
    }
}
