namespace CloneMessengerBackendAPI.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusReadMessage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserReadMessageInfor",
                c => new
                    {
                        ChatMessageId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ChatMessageId, t.UserId })
                .ForeignKey("dbo.ChatMessage", t => t.ChatMessageId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.ChatMessageId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.ChatMember", "IsRemoved", c => c.Boolean(nullable: false));
            DropColumn("dbo.ChatGroup", "UserIds");
            DropColumn("dbo.ChatGroup", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChatGroup", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.ChatGroup", "UserIds", c => c.String(nullable: false));
            DropForeignKey("dbo.UserReadMessageInfor", "UserId", "dbo.User");
            DropForeignKey("dbo.UserReadMessageInfor", "ChatMessageId", "dbo.ChatMessage");
            DropIndex("dbo.UserReadMessageInfor", new[] { "UserId" });
            DropIndex("dbo.UserReadMessageInfor", new[] { "ChatMessageId" });
            DropColumn("dbo.ChatMember", "IsRemoved");
            DropTable("dbo.UserReadMessageInfor");
        }
    }
}
