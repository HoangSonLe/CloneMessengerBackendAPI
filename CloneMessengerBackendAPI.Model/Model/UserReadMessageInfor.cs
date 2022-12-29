using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace CloneMessengerBackendAPI.Model.Model
{
    
    [Table("UserReadMessageInfor")]
    public partial class UserReadMessageInfor
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey(nameof(ChatMessage))]
        public Guid ChatMessageId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual ChatMessage ChatMessage { get; set; }
        public virtual User User { get; set; }
    }
}
