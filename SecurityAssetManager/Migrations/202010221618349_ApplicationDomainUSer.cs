namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationDomainUSer : DbMigration
    {
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserDomains", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserDomains", "Domain_DomainID", "dbo.Domains");
            DropIndex("dbo.ApplicationUserDomains", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ApplicationUserDomains", new[] { "Domain_DomainID" });
            /*DropColumn("dbo.Locations", "StreetName");
            DropColumn("dbo.Locations", "StreetNum");
            DropColumn("dbo.Locations", "City");
            DropColumn("dbo.Locations", "State");
            DropColumn("dbo.Locations", "Country");*/
            DropTable("dbo.ApplicationUserDomains");
        }
        
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserDomains",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Domain_DomainID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Domain_DomainID });
            
            /*AddColumn("dbo.Locations", "Country", c => c.String(nullable: false));
            AddColumn("dbo.Locations", "State", c => c.String(nullable: false));
            AddColumn("dbo.Locations", "City", c => c.String(nullable: false));
            AddColumn("dbo.Locations", "StreetNum", c => c.String(nullable: false));
            AddColumn("dbo.Locations", "StreetName", c => c.String(nullable: false));*/
            CreateIndex("dbo.ApplicationUserDomains", "Domain_DomainID");
            CreateIndex("dbo.ApplicationUserDomains", "ApplicationUser_Id");
            AddForeignKey("dbo.ApplicationUserDomains", "Domain_DomainID", "dbo.Domains", "DomainID", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserDomains", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
