namespace CloneMessengerBackendAPI.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChatTextMessage")]
    public partial class ChatTextMessage
    {
        [Key]
        public Guid ChatMessageId { get; set; }

        public string Text { get; set; }

        public virtual ChatMessage ChatMessage { get; set; }
    }
}
