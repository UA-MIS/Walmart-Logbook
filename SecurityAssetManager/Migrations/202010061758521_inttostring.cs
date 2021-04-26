namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inttostring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Domains", "DomainName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Domains", "DomainName", c => c.Int(nullable: false));
        }
    }
}
