namespace CloneMessengerBackendAPI.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileStructureDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileAttachment",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Ext = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.Guid(),
                        Data = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.CreatedBy)
                .Index(t => t.CreatedBy);
            
            AddColumn("dbo.User", "AvatarFileId", c => c.Guid());
            CreateIndex("dbo.ChatFileAttachment", "FileId");
            CreateIndex("dbo.User", "AvatarFileId");
            AddForeignKey("dbo.User", "AvatarFileId", "dbo.FileAttachment", "Id");
            AddForeignKey("dbo.ChatFileAttachment", "FileId", "dbo.FileAttachment", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatFileAttachment", "FileId", "dbo.FileAttachment");
            DropForeignKey("dbo.User", "AvatarFileId", "dbo.FileAttachment");
            DropForeignKey("dbo.FileAttachment", "CreatedBy", "dbo.User");
            DropIndex("dbo.FileAttachment", new[] { "CreatedBy" });
            DropIndex("dbo.User", new[] { "AvatarFileId" });
            DropIndex("dbo.ChatFileAttachment", new[] { "FileId" });
            DropColumn("dbo.User", "AvatarFileId");
            DropTable("dbo.FileAttachment");
        }
    }
}
