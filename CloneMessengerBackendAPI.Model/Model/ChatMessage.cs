namespace CloneMessengerBackendAPI.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChatMessage")]
    public partial class ChatMessage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChatMessage()
        {
            ChatFileAttachments = new HashSet<ChatFileAttachment>();
        }
        public Guid Id { get; set; }

        [ForeignKey(nameof(ChatGroup))]
        public Guid GroupId { get; set; }

        [ForeignKey(nameof(User))]
        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsSystem { get; set; }
        public Guid ContinuityKeyByUser { get; set; }
        public Guid ContinuityKeyByTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatFileAttachment> ChatFileAttachments { get; set; }


        public virtual ChatGroup ChatGroup { get; set; }

        public virtual ChatTextMessage ChatTextMessage { get; set; }

        public virtual User User { get; set; }
    }
}
