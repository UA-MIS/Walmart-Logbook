namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedTravelBox : DbMigration
    {
        public override void Up()
        {
            Sql(@" 

   INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'834eb794-1aca-4e68-b3bb-a56a4f04e77b', N'travelbox@walmart.com', 0, N'AHTYisRizmrSSXfzBC2UrgI8H6q2mz3JzuqVFz+uYvCwalKhRpHBmL7e3ZNjWblztQ==', N'14bdac10-78ee-4d3f-85fa-74c94b3c3074', NULL, 0, 0, NULL, 1, 0, N'travelbox@walmart.com')

   INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'834eb794-1aca-4e68-b3bb-a56a4f04e77b', N'372cb0e4-c3ea-4ce9-98b3-26b79daecf8e')

        

            ");
        }
        
        public override void Down()
        {
        }
    }
}
