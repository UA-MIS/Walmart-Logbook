namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSelectedAttributeToUserDomains : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserDomains", "Selected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDomains", "Selected");
        }
    }
}
