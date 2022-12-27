namespace CloneMessengerBackendAPI.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatFileAttachment",
                c => new
                    {
                        ChatMessageId = c.Guid(nullable: false),
                        FileId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChatMessageId, t.FileId })
                .ForeignKey("dbo.ChatMessage", t => t.ChatMessageId, cascadeDelete: true)
                .Index(t => t.ChatMessageId);
            
            CreateTable(
                "dbo.ChatMessage",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GroupId = c.Guid(nullable: false),
                        CreatedBy = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChatGroup", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.CreatedBy, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.CreatedBy);
            
            CreateTable(
                "dbo.ChatGroup",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CreatedBy = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastMessageId = c.Guid(),
                        UserIds = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChatMessage", t => t.LastMessageId)
                .ForeignKey("dbo.User", t => t.CreatedBy)
                .Index(t => t.CreatedBy)
                .Index(t => t.LastMessageId);
            
            CreateTable(
                "dbo.ChatMember",
                c => new
                    {
                        ChatGroupId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        AddedBy = c.Guid(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChatGroupId, t.UserId })
                .ForeignKey("dbo.User", t => t.AddedBy)
                .ForeignKey("dbo.ChatGroup", t => t.ChatGroupId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.ChatGroupId)
                .Index(t => t.UserId)
                .Index(t => t.AddedBy);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserName = c.String(maxLength: 8000, unicode: false),
                        MD5Password = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false),
                        DisplayName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserLastReadMessage",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        ChatGroupId = c.Guid(nullable: false),
                        LastReadMessageId = c.Guid(nullable: false),
                        Time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ChatGroupId })
                .ForeignKey("dbo.ChatGroup", t => t.ChatGroupId)
                .ForeignKey("dbo.ChatMessage", t => t.LastReadMessageId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ChatGroupId)
                .Index(t => t.LastReadMessageId);
            
            CreateTable(
                "dbo.ChatTextMessage",
                c => new
                    {
                        ChatMessageId = c.Guid(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.ChatMessageId)
                .ForeignKey("dbo.ChatMessage", t => t.ChatMessageId)
                .Index(t => t.ChatMessageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatFileAttachment", "ChatMessageId", "dbo.ChatMessage");
            DropForeignKey("dbo.ChatMessage", "CreatedBy", "dbo.User");
            DropForeignKey("dbo.ChatTextMessage", "ChatMessageId", "dbo.ChatMessage");
            DropForeignKey("dbo.ChatMessage", "GroupId", "dbo.ChatGroup");
            DropForeignKey("dbo.ChatGroup", "CreatedBy", "dbo.User");
            DropForeignKey("dbo.ChatGroup", "LastMessageId", "dbo.ChatMessage");
            DropForeignKey("dbo.ChatMember", "UserId", "dbo.User");
            DropForeignKey("dbo.ChatMember", "ChatGroupId", "dbo.ChatGroup");
            DropForeignKey("dbo.ChatMember", "AddedBy", "dbo.User");
            DropForeignKey("dbo.UserLastReadMessage", "UserId", "dbo.User");
            DropForeignKey("dbo.UserLastReadMessage", "LastReadMessageId", "dbo.ChatMessage");
            DropForeignKey("dbo.UserLastReadMessage", "ChatGroupId", "dbo.ChatGroup");
            DropIndex("dbo.ChatTextMessage", new[] { "ChatMessageId" });
            DropIndex("dbo.UserLastReadMessage", new[] { "LastReadMessageId" });
            DropIndex("dbo.UserLastReadMessage", new[] { "ChatGroupId" });
            DropIndex("dbo.UserLastReadMessage", new[] { "UserId" });
            DropIndex("dbo.ChatMember", new[] { "AddedBy" });
            DropIndex("dbo.ChatMember", new[] { "UserId" });
            DropIndex("dbo.ChatMember", new[] { "ChatGroupId" });
            DropIndex("dbo.ChatGroup", new[] { "LastMessageId" });
            DropIndex("dbo.ChatGroup", new[] { "CreatedBy" });
            DropIndex("dbo.ChatMessage", new[] { "CreatedBy" });
            DropIndex("dbo.ChatMessage", new[] { "GroupId" });
            DropIndex("dbo.ChatFileAttachment", new[] { "ChatMessageId" });
            DropTable("dbo.ChatTextMessage");
            DropTable("dbo.UserLastReadMessage");
            DropTable("dbo.User");
            DropTable("dbo.ChatMember");
            DropTable("dbo.ChatGroup");
            DropTable("dbo.ChatMessage");
            DropTable("dbo.ChatFileAttachment");
        }
    }
}
