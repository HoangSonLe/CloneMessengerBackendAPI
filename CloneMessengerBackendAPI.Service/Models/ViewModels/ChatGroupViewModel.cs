using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class ChatGroupViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? LastMessageId { get; set; }

        public string UserIds { get; set; }

        public int Status { get; set; }
        public bool IsRead { get; set; }
        public ChatMessageViewModel LastMessage { get; set; } //thông tin tin nhắn cuối cùng
    }

    public class CreateChatGroupModel
    {
        public List<UserViewModel> Users { get; set; }
        public ChatMessagePostData ChatMessageData { get; set; }
    }
}
