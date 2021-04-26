namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stringLens : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "Name", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Items", "Description", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Items", "Description", c => c.String());
            AlterColumn("dbo.Items", "Name", c => c.String(nullable: false));
        }
    }
}
