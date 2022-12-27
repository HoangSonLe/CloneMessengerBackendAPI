namespace CloneMessengerBackendAPI.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChatMember")]
    public partial class ChatMember
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey(nameof(ChatGroup))]
        public Guid ChatGroupId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(AddedUser))]
        public Guid AddedBy { get; set; }

        public DateTime AddedDate { get; set; }

        public virtual ChatGroup ChatGroup { get; set; }
        public virtual User User { get; set; }
        public virtual User AddedUser { get; set; }
    }
}
