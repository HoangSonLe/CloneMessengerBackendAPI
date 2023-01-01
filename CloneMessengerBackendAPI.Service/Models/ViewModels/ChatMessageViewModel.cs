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
        public string Text { get; set; }
    }
    /// <summary>
    /// Group by time with a setting preiod time
    /// </summary>
    public class ChatMessageGroupByTimeViewModel
    {
        public Guid ContinuityKeyByTime { get; set; }
        public DateTime GroupMessageTime { get; set; }
        public List<ChatMessageGroupByUserViewModel> GroupMessageListByUser { get; set; }
    }
    /// <summary>
    /// Group messages by user
    /// </summary>
    public class ChatMessageGroupByUserViewModel
    {
        public Guid ContinuityKeyByUser { get; set; }
        public bool IsMyMessage { get; set; }
        public List<ChatMessageViewModel> Messages { get; set; }

    }
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
