using CloneMessengerBackendAPI.Service.Helper;
using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Models;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Serviecs
{
    public interface IMessageServices
    {
        Task<Acknowledgement<List<ChatGroupViewModel>>> GetChatGroups(ChatMessageFilterModel filter);

        Task<Acknowledgement<ChatGroupViewModel>> GetChatMessageDetail(Guid chatGroupId);


    }
    public class MessageService : BaseService,IMessageServices
    {
        public Guid CurrentUserId()
        {
            return Guid.Parse("29CA1C9B-04AF-45CE-A5D9-DC7849A35EBC");
        }
        public async Task<Acknowledgement<List<ChatGroupViewModel>>> GetChatGroups(ChatMessageFilterModel filter)
        {
            var currentUserId = CurrentUserId();
            var context = DbContext;

            var seftGroupIds = context.ChatMembers.Where(i => i.UserId == currentUserId).Select(i => i.ChatGroupId);
            var queryGroup = context.ChatGroups.Where(i => seftGroupIds.Contains(i.Id));
            var query = (from gr in queryGroup
                         join lm in context.UserLastReadMessages.Where(i => i.UserId == currentUserId)
                         on gr.Id equals lm.ChatGroupId into lm1
                         from lm in lm1.DefaultIfEmpty()
                         select new
                         {
                             ChatGroup = gr,
                             LastMessage = gr.LastChatMessage,
                             LastReadMessage = lm,
                             ChatTextMessage = gr.LastChatMessage.ChatTextMessage,
                         }).OrderByDescending(i => i.LastMessage != null ? i.LastMessage.CreatedDate : i.ChatGroup.CreatedDate).AsQueryable();

            if (filter.PageSize.HasValue)
            {
                query = query.Skip(filter.Skip).Take(filter.PageSize.Value);
            }
            var groupData = await query.ToListAsync();

            var users =await (from g in queryGroup
                         join cm in context.ChatMembers on g.Id equals cm.ChatGroupId
                         join u in context.Users on cm.UserId equals u.Id
                         select u).ToListAsync();

            var result = new List<ChatGroupViewModel>();

            foreach (var g in groupData)
            {
                var gr = g.ChatGroup;
                var lrm = g.LastReadMessage;
                var lm = g.LastMessage;
                var group = new ChatGroupViewModel()
                {
                    Id = gr.Id,
                    Name = gr.Name,
                    CreatedDate = gr.CreatedDate,
                    CreatedBy = gr.CreatedBy,
                    LastMessageId = gr.LastMessageId,
                    Status = gr.Status,
                    UserIds = gr.UserIds,
                    IsRead = lrm == null ? true : lrm.LastReadMessageId == gr.LastChatMessage.Id,
                };
                if (lm != null)
                {
                    var u = users.First(i => i.Id == g.LastMessage.CreatedBy);
                    group.LastMessage = new ChatMessageViewModel()
                    {
                        Id = lm.Id,
                        IsSystem = lm.IsSystem,
                        Text = lm.ChatTextMessage.Text,
                        CreatedByName = u.DisplayName,
                        CreatedBy = lm.CreatedBy,
                        CreatedDate = lm.CreatedDate,
                    };
                }
                result.Add(group);
            }
            return new Acknowledgement<List<ChatGroupViewModel>>()
            {
                IsSuccess = true,
                Data = result
            };
        }
        public async Task<Acknowledgement> ReadLastMessage(Guid chatGroupId)
        {
            var ack = new Acknowledgement();
            var context = DbContext;
            var lastMessage = await (from g in context.ChatGroups.Where(i => i.Id == chatGroupId)
                                     join cm in context.ChatMessages on g.LastMessageId equals cm.Id
                                     select cm).FirstOrDefaultAsync();
            var lastMessage1 = await context.ChatGroups.Where(i => i.Id == chatGroupId)
                                       .Select(i => i.LastChatMessage)
                                       .FirstOrDefaultAsync();
            var currentUserId = CurrentUserId();
            if (lastMessage != null)
            {
                var dbData = await context.UserLastReadMessages.FirstOrDefaultAsync(i => i.UserId == currentUserId && i.ChatGroupId == chatGroupId);
                if (dbData == null)
                {
                    //thêm mới
                    var newData = new UserLastReadMessage()
                    {
                        ChatGroupId = chatGroupId,
                        LastReadMessageId = lastMessage.Id,
                        Time = DateTime.Now,
                        UserId = currentUserId
                    };
                    context.UserLastReadMessages.Add(newData);
                }
                else
                {
                    //update cũ
                    dbData.LastReadMessageId = lastMessage.Id;
                    dbData.Time = DateTime.Now;
                }
                ack.IsSuccess = true;
                await ack.TrySaveChangesAsync(context);
                if (ack.IsSuccess == false)
                {
                    return ack;
                }
            }

            return ack;
        }
        public async Task<Acknowledgement<ChatGroupViewModel>> GetChatMessageDetail(Guid chatGroupId)
        {
            var context = DbContext;
            var ack = new Acknowledgement<ChatGroupViewModel>();
            //Read last message before load message detail
            var readLastAck = await ReadLastMessage(chatGroupId);
            if(readLastAck.IsSuccess == false)
            {
                ack.IsSuccess = readLastAck.IsSuccess;
                ack.ErrorMessage = readLastAck.ErrorMessage;
                return ack;
            }
            //Load data
            var queryG = await context.ChatGroups.Where(i => i.Id == chatGroupId)
                                             .Include(i => i.ChatMembers)
                                             .Include(i => i.ChatMembers.Select(j => j.User))
                                             .Include(i => i.ChatMessages)
                                             .Include(i => i.ChatMessages.Select(j=>j.ChatTextMessage))
                                             .Include(i => i.UserLastReadMessages).ToListAsync();
            

            return ack;
        }

        public User MockData()
        {
            var users = new List<User>()
            {
                new User()
            {
                Id =  Guid.NewGuid(),
                UserName = "Hoang Huy 1",
                MD5Password = StringHelper.ToMD5Byte("1234"),
                CreatedDate = DateTime.Now,
                DisplayName = $"Hoang Huy {(new Random()).Next(1, 10)}"
            },
               new User()
            {
                Id =  Guid.NewGuid(),
                UserName = "Hoang Huy 2",
                MD5Password = StringHelper.ToMD5Byte("1234"),
                CreatedDate = DateTime.Now,
                DisplayName = $"Hoang Huy {(new Random()).Next(1, 10)}"
            }
            }
            ;
            var chatGroup = new ChatGroup()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                CreatedBy = users[0].Id,
                CreatedDate = DateTime.Now,
                UserIds = users[0].Id.ToString(),
                Status = (int)EChatGroupStatus.InGroup,
            };
            var chatMembers = new List<ChatMember>()
            {
                new ChatMember()
                {
                    ChatGroupId = chatGroup.Id,
                    UserId = users[0].Id,
                    AddedBy = users[0].Id,
                    AddedDate = DateTime.Now,
                },
                new ChatMember()
                {
                    ChatGroupId = chatGroup.Id,
                    UserId = users[1].Id,
                    AddedBy = users[0].Id,
                    AddedDate = DateTime.Now,
                }
            };

            var messages = new List<ChatMessage>()
            {
                new ChatMessage()
                {
                Id = Guid.NewGuid(),
                GroupId = chatGroup.Id,
                CreatedBy = users[0].Id,
                CreatedDate = DateTime.Now,
                IsSystem = false,
                ChatTextMessage = new ChatTextMessage()
                {
                    Text = "Test TestTest Test Test Test ",
                }
                },
                new ChatMessage()
                {
                    Id = Guid.NewGuid(),
                    GroupId = chatGroup.Id,
                    CreatedBy = users[1].Id,
                    CreatedDate = DateTime.Now,
                    IsSystem = false,
                    ChatTextMessage = new ChatTextMessage()
                    {
                         Text = "Test TestTest Test Test Test ",
                    }
                }
        };
            DbContext.Users.AddRange(users);
            DbContext.ChatGroups.Add(chatGroup);
            DbContext.ChatMembers.AddRange(chatMembers);
            DbContext.ChatMessages.AddRange(messages);
            DbContext.SaveChanges();
            return users[0];
        }
    }

}
