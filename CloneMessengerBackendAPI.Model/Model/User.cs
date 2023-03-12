namespace CloneMessengerBackendAPI.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            ChatGroups = new HashSet<ChatGroup>();
            ChatMessages = new HashSet<ChatMessage>();
            UserLastReadMessages = new HashSet<UserLastReadMessage>();
        }
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        public string UserName { get; set; }
        public byte[] MD5Password { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DisplayName { get; set; }

        [ForeignKey(nameof(AvatarFileAttachment))]
        public Guid? AvatarFileId { get; set; }

        public virtual FileAttachment AvatarFileAttachment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatGroup> ChatGroups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLastReadMessage> UserLastReadMessages { get; set; }
    }
}
