namespace CloneMessengerBackendAPI.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDatabaseChatGroup : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.ChatGroup", "ChatMessage_Id", "dbo.ChatMessage");
            //DropIndex("dbo.ChatGroup", new[] { "ChatMessage_Id" });
            //DropColumn("dbo.ChatGroup", "ChatMessage_Id");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.ChatGroup", "ChatMessage_Id", c => c.Guid());
            //CreateIndex("dbo.ChatGroup", "ChatMessage_Id");
            //AddForeignKey("dbo.ChatGroup", "ChatMessage_Id", "dbo.ChatMessage", "Id");
        }
    }
}
