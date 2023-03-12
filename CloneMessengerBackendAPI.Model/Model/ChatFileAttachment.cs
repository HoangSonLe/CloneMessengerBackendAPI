namespace CloneMessengerBackendAPI.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChatFileAttachment")]
    public partial class ChatFileAttachment
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey(nameof(ChatMessage))]
        public Guid ChatMessageId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey(nameof(FileAttachment))]
        public Guid FileId { get; set; }

        public virtual ChatMessage ChatMessage { get; set; }
        public virtual FileAttachment FileAttachment { get; set; }
    }
}
