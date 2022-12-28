using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class ChatGroupDetailViewModel 
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public List<ChatMessageGroupViewModel> MessageList { get; set; }
        public List<ChatMemberViewModel> ListMembers { get; set; }
        public ChatMessage DefaultChatMessage { get; set; }
        public ChatGroupDetailViewModel()
        {
            DefaultChatMessage = new ChatMessage()
            {
                GroupId = Id,
            };
        }

    }
    public class ChatMessagePostData : ChatMessage
    {
        public string Text { get; set; }
    }
    public class ChatMessageGroupViewModel
    {
        public DateTime GroupMessageTime { get; set;}
        public List<ChatMessageViewModel> Messages { get; set; }

    }
    public class ChatMessageViewModel : ChatMessage
    {
        public string CreatedByName { get; set; }
        public string Text { get; set;}
    }
    public class ChatMemberViewModel
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; }
        public bool IsOnline { get; set; }
        public string AddByName { get; set; }


    }
}
