namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userdomains : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationUserDomains", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserDomains", "Domain_DomainID", "dbo.Domains");
            DropIndex("dbo.ApplicationUserDomains", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserDomains", new[] { "Domain_DomainID" });
            CreateTable(
                "dbo.UserDomains",
                c => new
                    {
                        UserID = c.Guid(nullable: false),
                        DomainID = c.Guid(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserID, t.DomainID })
                .ForeignKey("dbo.Domains", t => t.DomainID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.DomainID)
                .Index(t => t.User_Id);
            
            DropTable("dbo.ApplicationUserDomains");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationUserDomains",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Domain_DomainID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Domain_DomainID });
            
            DropForeignKey("dbo.UserDomains", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserDomains", "DomainID", "dbo.Domains");
            DropIndex("dbo.UserDomains", new[] { "User_Id" });
            DropIndex("dbo.UserDomains", new[] { "DomainID" });
            DropTable("dbo.UserDomains");
            CreateIndex("dbo.ApplicationUserDomains", "Domain_DomainID");
            CreateIndex("dbo.ApplicationUserDomains", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserDomains", "Domain_DomainID", "dbo.Domains", "DomainID", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserDomains", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
