using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Models.SignalRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class ChatGroupDetailViewModel : BaseChatGroupView
    {
        public MessageStatus MessageStatus { get; set; }
        //public List<ChatMessageViewModel> MessageList { get; set; }
        public PaginationModel<List<ChatMessageGroupByTimeViewModel>> GroupMessageListByTime { get; set; }
        public ChatMessagePostData DefaultChatMessage { get; set; }
        
    }
}
