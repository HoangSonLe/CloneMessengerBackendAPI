using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class ChatGroupViewModel : ChatGroup
    {
        public bool IsRead { get; set; }
        public ChatMessageViewModel LastMessage { get; set; } //thông tin tin nhắn cuối cùng
    }
}
