using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Helper
{
    public static class DTOMapper
    {
        public static void MapDTOChatGroup(this ChatGroupViewModel vm,ChatGroup g)
        {
            vm.Id = g.Id;
            vm.Name = g.Name;
            vm.CreatedBy = g.CreatedBy;
            vm.CreatedDate = g.CreatedDate;
            vm.LastMessageId = g.LastMessageId;
        }
        public static void MapDTOChatMessage(this ChatMessageViewModel vm, ChatMessage m)
        {
            vm.Id = m.Id;
            vm.GroupId = m.Id;
            vm.CreatedBy = m.CreatedBy;
            vm.CreatedDate = m.CreatedDate;
            vm.IsSystem = m.IsSystem;
            vm.Text = m?.ChatTextMessage != null ? m.ChatTextMessage.Text : string.Empty;
        }
        public static void MapDTOUser(this UserViewModel vm, User u)
        {
            vm.Id = u.Id;
            vm.DisplayName= u.DisplayName;
        }
    }
}
