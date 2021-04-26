namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationDBV2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Containers",
                c => new
                    {
                        ContainerID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ContainerID);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        EventID = c.Guid(nullable: false),
                        EventCode = c.Int(nullable: false),
                        EventDescription = c.String(nullable: false),
                        Justification = c.String(nullable: false),
                        Action = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.EventID);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationID = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        StreetName = c.String(nullable: false),
                        StreetNum = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Country = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.LocationID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Locations");
            DropTable("dbo.Events");
            DropTable("dbo.Containers");
        }
    }
}
