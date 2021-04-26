namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Domains : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Domains",
                c => new
                    {
                        DomainID = c.Guid(nullable: false),
                        DomainName = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DomainID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Domains");
        }
    }
}
