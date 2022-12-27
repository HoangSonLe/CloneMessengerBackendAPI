namespace CloneMessengerBackendAPI.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserLastReadMessage")]
    public partial class UserLastReadMessage
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey(nameof(ChatGroup))]
        public Guid ChatGroupId { get; set; }

        [ForeignKey(nameof(ChatMessage))]

        public Guid LastReadMessageId { get; set; }

        public DateTime Time { get; set; }

        public virtual ChatMessage ChatMessage { get; set; }
        public virtual ChatGroup ChatGroup { get; set; }
        public virtual User User { get; set; }
    }
}
