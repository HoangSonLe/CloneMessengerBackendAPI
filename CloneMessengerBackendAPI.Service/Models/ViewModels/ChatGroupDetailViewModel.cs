using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class ChatGroupDetailViewModel 
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public bool IsGroup { get; }
        public bool IsRemoved { get; set; }
        //public List<ChatMessageViewModel> MessageList { get; set; }
        public PaginationModel<List<ChatMessageGroupByTimeViewModel>> GroupMessageListByTime { get; set; }
        public List<ChatMemberViewModel> ListMembers { get; set; }
        public ChatMessagePostData DefaultChatMessage { get; set; }
        public ChatGroupDetailViewModel()
        {
            ListMembers = new List<ChatMemberViewModel>();
            IsGroup = ListMembers.Count() > 2;
        }

    }
}
