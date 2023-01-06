using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.SignalRModels
{
    public class MessageSignalRModel
    {
        public bool IsNewGroupByUser { get; set; }
        public bool IsNewGroupByTime { get; set; }
        public ChatMessageGroupByTimeViewModel MessageGroupByTime { get; set; }
        public ChatMessageGroupByUserViewModel MessageGroupByUser { get; set; }
    }
    public class MessageSignalRWithStatus
    {
        public Guid GroupId { get; set; }

        public Guid KeyGroupByTime { get; set; }

        public Guid KeyGroupByUser { get; set; }

        public Guid MessageId { get; set; }
        public EMessageStatus Status { get; set; }  
        public List<Guid> UserReadMessageList { get; set; }
        public MessageSignalRWithStatus()
        {
            UserReadMessageList = new List<Guid>();
            Status = EMessageStatus.Sent;
        }
    }
}
