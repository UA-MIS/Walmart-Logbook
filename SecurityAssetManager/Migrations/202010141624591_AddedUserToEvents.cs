namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserToEvents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "User", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "User");
        }
    }
}
