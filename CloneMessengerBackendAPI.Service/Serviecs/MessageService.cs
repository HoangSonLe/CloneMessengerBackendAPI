using CloneMessengerBackendAPI.Model.Helper;
using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Models;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Serviecs
{
    public interface IMessageServices
    {
        Task<List<ChatGroupViewModel>> GetChatGroups(ChatMessageFilterModel filter);
        Task<List<ChatGroupViewModel>> GetChatMessageDetail(Guid Id);

    }
    public class MessageService : IMessageServices
    {
        private CloneMessengerDbContext DbContext = new CloneMessengerDbContext();
        public async Task<List<ChatGroupViewModel>> GetChatGroups(ChatMessageFilterModel filter)
        {
            var currenUserId = Guid.Parse("6c6390b5-e1c8-46f2-bbff-5f22760a3df9");

            var seftGroupIds = DbContext.ChatMembers.Where(i => i.UserId == currenUserId).Select(i => i.ChatGroupId);
            var queryGroup = DbContext.ChatGroups.Where(i => seftGroupIds.Contains(i.Id));
            var query = (from gr in queryGroup
                         join lm in DbContext.UserLastReadMessages.Where(i => i.UserId == currenUserId)
                         on gr.Id equals lm.ChatGroupId into lm1
                         from lm in lm1.DefaultIfEmpty()
                         select new
                         {
                             ChatGroup = gr,
                             LastMessage = gr.LastChatMessage,
                             LastMessageUser = gr.LastChatMessage.User,
                             LastReadMessage = lm,
                             ChatTextMessage = gr.LastChatMessage.ChatTextMessage,
                         }).OrderByDescending(i => i.LastMessage != null ? i.LastMessage.CreatedDate : i.ChatGroup.CreatedDate).AsQueryable();

            if (filter.PageSize.HasValue)
            {
                query = query.Skip(filter.Skip).Take(filter.PageSize.Value);
            }
            var groupData = await query.ToListAsync();

            var result = new List<ChatGroupViewModel>();

            foreach (var g in groupData) {
                var gr = g.ChatGroup;
                var lrm = g.LastReadMessage;
                var lm = g.LastMessage;
                var u = g.LastMessageUser;
                var group = new ChatGroupViewModel()
                {
                    Id = gr.Id,
                    Name = gr.Name,
                    CreatedDate = gr.CreatedDate,
                    IsRead = lrm == null ? true : lrm.LastReadMessageId == gr.LastChatMessage.Id,
                    LastMessage = lm == null ? null : new ChatMessageViewModel()
                    {
                        Id = lm.Id,
                        IsSystem = lm.IsSystem,
                        Text = lm.ChatTextMessage.Text,
                        CreatedByName = u.DisplayName,
                        CreatedBy = lm.CreatedBy,
                        CreatedDate = lm.CreatedDate,
                    }
                };
                result.Add(group);
            }
            return result;
        }

        public Task<List<ChatGroupViewModel>> GetChatMessageDetail(Guid Id)
        {
            throw new NotImplementedException();
        }

        public void MockData()
        {
            var userId = Guid.NewGuid();
            var user = new User()
            {
                Id = userId,
                UserName = "Hoang Huy",
                MD5Password = StringHelper.ToMD5Byte("1234"),
                CreatedDate = DateTime.Now,
                DisplayName = $"Hoang Huy {(new Random()).Next(1,10)}"
            };
            var chatGroup = new ChatGroup()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                CreatedBy = user.Id,
                CreatedDate = DateTime.Now,
                UserIds = user.Id.ToString(),
                Status = (int)EChatGroupStatus.InGroup,
            };
            var chatMember = new ChatMember()
            {
                ChatGroupId = chatGroup.Id,
                UserId = userId,
                AddedBy = user.Id,
                AddedDate = DateTime.Now,
            };

            var message = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                GroupId = chatGroup.Id,
                CreatedBy = user.Id,
                CreatedDate = DateTime.Now,
                IsSystem = false,
                ChatTextMessage = new ChatTextMessage()
                {
                    Text = "Test",
                }
            };
            DbContext.ChatMembers.Add(chatMember);
            //DbContext.Users.Add(user);
            DbContext.ChatGroups.Add(chatGroup);
            DbContext.ChatMessages.Add(message);
            DbContext.SaveChanges();
        }
    }
  
}
