using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace CloneMessengerBackendAPI.Model.Model
{
    public partial class CloneMessengerDbContext : DbContext
    {
        public CloneMessengerDbContext()
            : base("name=CloneMessengerDataContext")
        {
#if DEBUG
            this.Database.Log = (s) => { Debug.WriteLine(s); };
#endif
        }

        public virtual DbSet<ChatFileAttachment> ChatFileAttachments { get; set; }
        public virtual DbSet<ChatGroup> ChatGroups { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ChatMember> ChatMembers { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<ChatTextMessage> ChatTextMessages { get; set; }
        public virtual DbSet<FileAttachment> FileAttachments { get; set; }  
        public virtual DbSet<UserLastReadMessage> UserLastReadMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatGroup>()
                .HasOptional(e => e.LastChatMessage)
                .WithMany().HasForeignKey(e=>e.LastMessageId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatGroup>()
                .HasRequired(e => e.User)
                .WithMany(e => e.ChatGroups)
                .HasForeignKey(e => e.CreatedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatMember>()
                .HasKey(e => new { e.UserId, e.ChatGroupId });

            modelBuilder.Entity<ChatMember>()
                .HasRequired(e => e.AddedUser)
                .WithMany()
                .HasForeignKey(e=>e.AddedBy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatMember>()
                .HasRequired(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserLastReadMessage>()
                .HasRequired(e => e.ChatGroup)
                .WithMany(e => e.UserLastReadMessages)
                .HasForeignKey(e=> e.ChatGroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserLastReadMessage>()
                .HasRequired(e => e.User)
                .WithMany(e => e.UserLastReadMessages)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasOptional<FileAttachment>(e => e.AvatarFileAttachment)
                //.WithOptionalDependent()
                .WithMany().HasForeignKey(e => e.AvatarFileId).WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //   .HasOptional<FileAttachment>(e => e.AvatarFileAttachment)
            //   .WithMany().HasForeignKey(e=> e.AvatarFileId).WillCascadeOnDelete(false);
        }
    }
}
