using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using Microsoft.AspNet.SignalR;
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
        public static void MapDTOChatGroup(this ChatGroupViewModel vm, ChatGroup g, ChatMessage lm, IChatHubService hub)
        {
            vm.Id = g.Id;
            vm.Name = g.Name;
            vm.CreatedBy = g.CreatedBy;
            vm.CreatedDate = g.CreatedDate;
            vm.LastMessageId = g.LastMessageId;
            vm.ListMembers = MapChatMemberViewModelList(g.ChatMembers.ToList(), hub);
            if (lm != null)
            {
                var u = g.ChatMembers.First(i => i.UserId == lm.CreatedBy).User;
                vm.LastMessage = new ChatMessageViewModel()
                {
                    CreatedByName = u.DisplayName,
                    MessageStatus = EMessageStatus.Sent,
                };
                vm.LastMessage.MapDTOChatMessage(lm);
            }
        }
        public static List<ChatMemberViewModel> MapChatMemberViewModelList(List<ChatMember> l,IChatHubService hub)
        {
            var userOnlineIds = hub.GetUserOnlines(l.Select(i => i.UserId).ToList());

            return l.Select(i =>
            {
                var m = i.User;
                var addBy = i.AddedUser;
                var t = new ChatMemberViewModel()
                {
                    UserId = m.Id,
                    DisplayName = m.DisplayName,
                    IsOnline = userOnlineIds.Contains(m.Id),
                    AddByName = addBy.DisplayName,
                    AvatarFileId = m.AvatarFileId
                };
                return t;
            }).ToList();
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
            vm.AvatarFileId = u.AvatarFileId;
            vm.DisplayName = u.DisplayName;
        }

    }
}
