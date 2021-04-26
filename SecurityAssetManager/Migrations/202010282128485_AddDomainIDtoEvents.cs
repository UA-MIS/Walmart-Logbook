namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDomainIDtoEvents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "DomainID", c => c.Guid());
        }
        
        public override void Down()
        {
          //  DropColumn("dbo.Events", "DomainID");
        }
    }
}
