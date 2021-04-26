namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seedRoles1 : DbMigration
    {
        public override void Up()
        {

            Sql(@"
        INSERT INTO[dbo].[AspNetRoles]
        ([Id], [Name]) VALUES(N'ffb9f2c4-077b-4708-bf6c-d8eedcd53dd7', N'Admin')
        INSERT INTO[dbo].[AspNetRoles]
        ([Id], [Name]) VALUES(N'3dba487d-bb36-41cb-a864-e27845d13f80', N'Auditor')
        INSERT INTO[dbo].[AspNetRoles]
        ([Id], [Name]) VALUES(N'372cb0e4-c3ea-4ce9-98b3-26b79daecf8e', N'Keyholder')
        INSERT INTO[dbo].[AspNetRoles]
        ([Id], [Name]) VALUES(N'45807eb7-8484-4bd6-81b2-ba1f4b5723f2', N'Witness')

        
        INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'a26e21e8-ab41-40db-b1a2-d99e4fb4fdd1', N'ffb9f2c4-077b-4708-bf6c-d8eedcd53dd7')





        ");
        }
        
        public override void Down()
        {
        }
    }
}
