namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "DateString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "DateString");
        }
    }
}
