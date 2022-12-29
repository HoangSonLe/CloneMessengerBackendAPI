using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class ChatMessagePostData
    {
        public Guid GroupId { get; set; }
        public Guid CurrentUserId { get; set; }
        public string Text { get; set; }
    }
    //public class ChatMessageGroupViewModel
    //{
    //    public DateTime GroupMessageTime { get; set;}
    //    public List<ChatMessageViewModel> Messages { get; set; }

    //}
    public class ChatMessageViewModel
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsSystem { get; set; }

        public string CreatedByName { get; set; }
        public string Text { get; set;}
    }
}
