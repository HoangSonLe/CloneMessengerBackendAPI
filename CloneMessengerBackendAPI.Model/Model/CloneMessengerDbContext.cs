using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CloneMessengerBackendAPI.Model.Model
{
    public partial class CloneMessengerDbContext : DbContext
    {
        public CloneMessengerDbContext()
            : base("name=CloneMessengerDataContext")
        {
        }

        public virtual DbSet<ChatFileAttachment> ChatFileAttachments { get; set; }
        public virtual DbSet<ChatGroup> ChatGroups { get; set; }
        public virtual DbSet<ChatMember> ChatMembers { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<ChatTextMessage> ChatTextMessages { get; set; }
        public virtual DbSet<UserLastReadMessage> UserLastReadMessages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatGroup>()
                .HasMany(e => e.ChatMessages)
                .WithRequired(e => e.ChatGroup)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<ChatGroup>()
                .HasOptional(e => e.ChatMessage)
                .WithMany(e => e.ChatGroups);

            modelBuilder.Entity<ChatGroup>()
                .HasRequired(e => e.User)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatMessage>()
                .HasOptional(e => e.ChatTextMessage)
                .WithRequired(e => e.ChatMessage)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ChatMember>()
                .HasRequired(e => e.AddedUser)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatMember>()
                .HasRequired(e => e.User)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(false);

        }
    }
}
