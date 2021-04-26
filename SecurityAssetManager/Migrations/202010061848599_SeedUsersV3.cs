namespace SecurityAssetManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedUsersV3 : DbMigration
    {
        public override void Up()
        {
            Sql(@" 


INSERT INTO [dbo].[Domains] ([DomainID], [DomainName]) VALUES (N'aa02fe37-5f79-47ca-be36-09fcc0d96d4a', N'TESTDOMAIN')

INSERT INTO[dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [DomainID]) VALUES(N'a26e21e8-ab41-40db-b1a2-d99e4fb4fdd1', N'admin1@test.com', 0, N'AG0sPRcJWdjo2s6mzr2SrXEoOBit1sCVtqSZ6CxUA1TdYAN84mhZUKSAxU8M4zavrw==', N'0be4d921-d16e-4071-9f56-5b7d22232192', NULL, 0, 0, NULL, 1, 0, N'admin1@test.com', N'aa02fe37-5f79-47ca-be36-09fcc0d96d4a')


INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [DomainID]) VALUES (N'4778f067-557c-4c08-a3a3-408c788c1a22', N'guest@test.com', 0, N'AL86hmWWQ8dwQqIFqgRiOreIMkWxbH6lB51G6lHznga+iO8/ApZLMlaF1IxWOHpAwg==', N'b7044799-177f-4659-96fc-4370ed5c8477', NULL, 0, 0, NULL, 1, 0, N'guest@test.com',N'aa02fe37-5f79-47ca-be36-09fcc0d96d4a')

");

        }

    public override void Down()
        {
        }
    }
}
