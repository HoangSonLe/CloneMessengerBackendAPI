namespace CloneMessengerBackendAPI.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusReadMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatMember", "IsRemoved", c => c.Boolean(nullable: false));
            DropColumn("dbo.ChatGroup", "UserIds");
            DropColumn("dbo.ChatGroup", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChatGroup", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.ChatGroup", "UserIds", c => c.String(nullable: false));
            DropColumn("dbo.ChatMember", "IsRemoved");
        }
    }
}
