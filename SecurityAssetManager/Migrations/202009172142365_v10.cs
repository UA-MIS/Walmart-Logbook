namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v10 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Events", "DateString");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "DateString", c => c.String());
        }
    }
}
