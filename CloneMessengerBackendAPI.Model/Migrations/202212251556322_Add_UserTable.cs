namespace CloneMessengerBackendAPI.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_UserTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(),
                        MD5Password = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false),
                        DisplayName = c.String(),
                        ChatMember_ChatGroupId = c.Guid(nullable: false),
                        ChatMember_UserId = c.Guid(nullable: false),
                        ChatMember_ChatGroupId1 = c.Guid(nullable: false),
                        ChatMember_UserId1 = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChatMember", t => new { t.ChatMember_ChatGroupId, t.ChatMember_UserId })
                .ForeignKey("dbo.ChatMember", t => new { t.ChatMember_ChatGroupId1, t.ChatMember_UserId1 })
                .ForeignKey("dbo.ChatGroup", t => t.Id)
                .Index(t => t.Id)
                .Index(t => new { t.ChatMember_ChatGroupId, t.ChatMember_UserId })
                .Index(t => new { t.ChatMember_ChatGroupId1, t.ChatMember_UserId1 });
            
            AddColumn("dbo.ChatGroup", "User_Id", c => c.Guid());
            CreateIndex("dbo.ChatMessage", "CreatedBy");
            CreateIndex("dbo.ChatGroup", "User_Id");
            CreateIndex("dbo.UserLastReadMessage", "UserId");
            AddForeignKey("dbo.ChatGroup", "User_Id", "dbo.User", "Id");
            AddForeignKey("dbo.UserLastReadMessage", "UserId", "dbo.User", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ChatMessage", "CreatedBy", "dbo.User", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatMessage", "CreatedBy", "dbo.User");
            DropForeignKey("dbo.User", "Id", "dbo.ChatGroup");
            DropForeignKey("dbo.User", new[] { "ChatMember_ChatGroupId1", "ChatMember_UserId1" }, "dbo.ChatMember");
            DropForeignKey("dbo.User", new[] { "ChatMember_ChatGroupId", "ChatMember_UserId" }, "dbo.ChatMember");
            DropForeignKey("dbo.UserLastReadMessage", "UserId", "dbo.User");
            DropForeignKey("dbo.ChatGroup", "User_Id", "dbo.User");
            DropIndex("dbo.UserLastReadMessage", new[] { "UserId" });
            DropIndex("dbo.User", new[] { "ChatMember_ChatGroupId1", "ChatMember_UserId1" });
            DropIndex("dbo.User", new[] { "ChatMember_ChatGroupId", "ChatMember_UserId" });
            DropIndex("dbo.User", new[] { "Id" });
            DropIndex("dbo.ChatGroup", new[] { "User_Id" });
            DropIndex("dbo.ChatMessage", new[] { "CreatedBy" });
            DropColumn("dbo.ChatGroup", "User_Id");
            DropTable("dbo.User");
        }
    }
}
