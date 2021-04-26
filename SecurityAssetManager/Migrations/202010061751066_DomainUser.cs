namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DomainUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Containers", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "DomainID", c => c.Guid(nullable: false));
            CreateIndex("dbo.Containers", "UserId");
            CreateIndex("dbo.AspNetUsers", "DomainID");
            AddForeignKey("dbo.AspNetUsers", "DomainID", "dbo.Domains", "DomainID", cascadeDelete: true);
            AddForeignKey("dbo.Containers", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Containers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "DomainID", "dbo.Domains");
            DropIndex("dbo.AspNetUsers", new[] { "DomainID" });
            DropIndex("dbo.Containers", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "DomainID");
            DropColumn("dbo.Containers", "UserId");
        }
    }
}
