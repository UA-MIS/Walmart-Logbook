namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewModelUpdates109 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "DomainID", "dbo.Domains");
            DropIndex("dbo.AspNetUsers", new[] { "DomainID" });
            CreateTable(
                "dbo.ApplicationUserDomains",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Domain_DomainID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Domain_DomainID })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Domains", t => t.Domain_DomainID, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Domain_DomainID);
            
            AddColumn("dbo.Containers", "DomainID", c => c.Guid(nullable: false));
            AddColumn("dbo.Domains", "Description", c => c.String());
            CreateIndex("dbo.Containers", "DomainID");
            AddForeignKey("dbo.Containers", "DomainID", "dbo.Domains", "DomainID", cascadeDelete: true);
            DropColumn("dbo.AspNetUsers", "DomainID");
            DropColumn("dbo.Items", "needWitness");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Items", "needWitness", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "DomainID", c => c.Guid(nullable: false));
            DropForeignKey("dbo.Containers", "DomainID", "dbo.Domains");
            DropForeignKey("dbo.ApplicationUserDomains", "Domain_DomainID", "dbo.Domains");
            DropForeignKey("dbo.ApplicationUserDomains", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserDomains", new[] { "Domain_DomainID" });
            DropIndex("dbo.ApplicationUserDomains", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Containers", new[] { "DomainID" });
            DropColumn("dbo.Domains", "Description");
            DropColumn("dbo.Containers", "DomainID");
            DropTable("dbo.ApplicationUserDomains");
            CreateIndex("dbo.AspNetUsers", "DomainID");
            AddForeignKey("dbo.AspNetUsers", "DomainID", "dbo.Domains", "DomainID", cascadeDelete: true);
        }
    }
}
