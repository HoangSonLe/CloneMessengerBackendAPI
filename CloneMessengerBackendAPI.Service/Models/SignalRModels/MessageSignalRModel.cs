using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.SignalRModels
{
    public class CreateConversationModel
    {
        public bool IsCreateUser { get; set; }
        public ChatGroupViewModel Group { get; set; }
        public ChatGroupDetailViewModel Conversation { get; set; }
    }
    public class MessageSignalRModel
    {
        public Guid ChatGroupId { get; set; }
        public bool IsNewGroupByUser { get; set; }
        public bool IsNewGroupByTime { get; set; }
        public ChatMessageGroupByTimeViewModel MessageGroupByTime { get; set; }
        public ChatMessageGroupByUserViewModel MessageGroupByUser { get; set; }
    }
    public class MessageStatus 
    { 
        public Guid ChatGroupId { get; set;}
        public List<MessageStatusItem> MessageStatusItemList { get; set; }

    }
    public class MessageStatusItem
    {
        public Guid ChatMessageId { get; set; }
        public DateTime ReadTime { get; set; }
        public DateTime LastMessageCreatedDate { get; set; }
        public Guid UserId { get; set; }
        public string DisplayName { get; set; }
    }
    public class MessageInforModel
    {
        public Guid ChatGroupId { get; set; }

        public Guid KeyGroupByTime { get; set; }

        public Guid KeyGroupByUser { get; set; }

        public Guid MessageId { get; set; }
        public EMessageStatus Status { get; set; }  
        public MessageInforModel()
        {
            Status = EMessageStatus.Sent;
        }
    }
}
