namespace CloneMessengerBackendAPI.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnContinuityKeyMessages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatMessage", "ContinuityKeyByUser", c => c.Guid(nullable: false));
            AddColumn("dbo.ChatMessage", "ContinuityKeyByTime", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatMessage", "ContinuityKeyByTime");
            DropColumn("dbo.ChatMessage", "ContinuityKeyByUser");
        }
    }
}
