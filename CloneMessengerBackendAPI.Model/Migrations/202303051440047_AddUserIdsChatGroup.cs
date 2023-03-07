namespace CloneMessengerBackendAPI.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserIdsChatGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatGroup", "UserIds", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatGroup", "UserIds");
        }
    }
}
