using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CloneMessengerBackendAPI.Model.Model
{
    [Table("FileAttachment")]
    public partial class FileAttachment
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public DateTime CreatedDate { get; set; }
        [ForeignKey(nameof(CreatedUser))]
        public Guid? CreatedBy { get; set; }
        public byte[] Data { get; set; }

        public virtual User CreatedUser { get; set; }
    }
}
