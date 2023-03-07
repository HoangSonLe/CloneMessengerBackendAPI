using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.SignalRModels;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class BaseChatGroupView
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsGroup { get; }
        public bool IsRemoved { get; set; }
        public bool IsTmp { get; set; }
        public List<ChatMemberViewModel> ListMembers { get; set; }
        public BaseChatGroupView()
        {
            ListMembers = new List<ChatMemberViewModel>();
            IsGroup = ListMembers.Count() > 2;
        }
    }
    public class ChatGroupViewModel : BaseChatGroupView
    {
        public Guid? LastMessageId { get; set; }

        public MessageStatus MessageStatus { get; set; }
        public ChatMessageViewModel LastMessage { get; set; } //thông tin tin nhắn cuối cùng
    }

    public class CreateChatGroupModel : BaseModelWithUserIdentity
    {
        public List<Guid> UserIds { get; set; }
        public string Text { get; set; }
    }
}
