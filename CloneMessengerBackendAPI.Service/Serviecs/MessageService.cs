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
using LinqKit;

namespace CloneMessengerBackendAPI.Service.Serviecs
{
    public interface IMessageServices
    {
        Task<Acknowledgement<List<ChatGroupViewModel>>> GetChatGroups(PaginationModel post);
        Task<Acknowledgement<ChatGroupDetailViewModel>> GetChatGroupDetail(ChatMessagePaginationModel post);
        Task<Acknowledgement> SendMessage(ChatMessagePostData post);
        Task<Acknowledgement<List<ChatMessageViewModel>>> GetMessageList(ChatMessagePaginationModel post);
        Task<Acknowledgement<List<UserViewModel>>> GetUserList(string searchValue);
        Task<Acknowledgement> CreateChatGroup(CreateChatGroupModel post);


    }
    public class MessageService : BaseService, IMessageServices
    {
        public Guid CurrentUserId()
        {
            return Guid.Parse("29CA1C9B-04AF-45CE-A5D9-DC7849A35EBC");
        }
        public bool GetCurrentStatusOnlineByUserId(Guid userId)
        {
            return true;
        }
        #region API
        /// <summary>
        /// Get list of chat group
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<List<ChatGroupViewModel>>> GetChatGroups(PaginationModel post)
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

            if (post.PageSize.HasValue)
            {
                query = query.Skip(post.Skip).Take(post.PageSize.Value);
            }
            var groupData = await query.ToListAsync();

            var users = await (from g in queryGroup
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
                    IsRead = lrm == null ? true : lrm.LastReadMessageId == gr.LastChatMessage.Id,
                };
                group.MapDTOChatGroup(gr);
                if (lm != null)
                {
                    var u = users.First(i => i.Id == g.LastMessage.CreatedBy);
                    group.LastMessage = new ChatMessageViewModel()
                    {
                        //Text = lm.ChatTextMessage.Text,
                        CreatedByName = u.DisplayName,
                    };
                    group.LastMessage.MapDTOChatMessage(lm);
                }
                result.Add(group);
            }
            return new Acknowledgement<List<ChatGroupViewModel>>()
            {
                IsSuccess = true,
                Data = result
            };
        }
        /// <summary>
        /// Update last read message of user
        /// </summary>
        /// <param name="chatGroupId"></param>
        /// <returns></returns>
        public async Task<Acknowledgement> ReadLastMessage(Guid chatGroupId)
        {
            var ack = new Acknowledgement();
            var context = DbContext;
            var lastMessage = await (from g in context.ChatGroups.Where(i => i.Id == chatGroupId)
                                     join cm in context.ChatMessages on g.LastMessageId equals cm.Id
                                     select cm).FirstOrDefaultAsync();
            var currentUserId = CurrentUserId();
            if (lastMessage != null)
            {
                var isNeedSave = true;
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
                    if (dbData.LastReadMessageId == lastMessage.Id)
                    {
                        isNeedSave = false;
                    }
                    else
                    {
                        dbData.LastReadMessageId = lastMessage.Id;
                        dbData.Time = DateTime.Now;
                    }
                }
                ack.IsSuccess = true;
                if (isNeedSave == true)
                {
                    await ack.TrySaveChangesAsync(context);
                }
                if (ack.IsSuccess == false)
                {
                    return ack;
                }
            }

            return ack;
        }
        /// <summary>
        /// Get list messages in group -- has pagination
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<List<ChatMessageViewModel>>> GetMessageList(ChatMessagePaginationModel post)
        {
            var context = DbContext;
            var ack = new Acknowledgement<List<ChatMessageViewModel>>();
            var messageQuery = context.ChatMessages.Where(i => i.GroupId == post.ChatGroupId)
                                               .Include(i => i.ChatTextMessage)
                                               .Include(i => i.User).OrderByDescending(i => i.CreatedDate).AsQueryable();
            if (post.PageSize.HasValue)
            {
                messageQuery = messageQuery.Skip(post.Skip).Take(post.PageSize.Value);
            }
            var messages = await messageQuery.ToArrayAsync();

            var result = messages.Select(i => MapChatMessageViewModel(i, i.User)).OrderBy(i => i.CreatedDate).ToList();
            ack.Data = result;
            ack.IsSuccess = true;
            return ack;

        }
        /// <summary>
        /// Get chat message detail (list messages, list members,...)
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<ChatGroupDetailViewModel>> GetChatGroupDetail(ChatMessagePaginationModel post)
        {
            var context = DbContext;
            var ack = new Acknowledgement<ChatGroupDetailViewModel>();
            //Read last message before load message detail
            var readLastAck = await ReadLastMessage(post.ChatGroupId);
            if (readLastAck.IsSuccess == false)
            {
                ack.IsSuccess = readLastAck.IsSuccess;
                ack.ErrorMessage = readLastAck.ErrorMessage;
                return ack;
            }
            //Load data
            var queryG = await context.ChatGroups.Where(i => i.Id == post.ChatGroupId)
                                             .Include(i => i.ChatMembers)
                                             .Include(i => i.ChatMembers.Select(j => j.User))
                                             .Include(i => i.ChatMembers.Select(j => j.AddedUser))
                                             .Include(i => i.UserLastReadMessages).FirstOrDefaultAsync();



            if (queryG == null)
            {
                ack.IsSuccess = false;
                ack.ErrorMessage = new List<string>() { "Chat group is not found" };
                return ack;
            }
            var messagesAck = await GetMessageList(post);
            var result = new ChatGroupDetailViewModel()
            {
                Id = queryG.Id,
                Name = queryG.Name,
                ListMembers = queryG.ChatMembers.Select(i => MapChatMemberViewModel(i.User, i.AddedUser)).ToList(),
                MessageList = messagesAck.Data,
                DefaultChatMessage = new ChatMessagePostData()
                {
                    GroupId = queryG.Id,
                    Text = string.Empty,
                    CurrentUserId = CurrentUserId()
                }
            };

            ack.Data = result;
            ack.IsSuccess = true;
            return ack;
        }
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Acknowledgement> SendMessage(ChatMessagePostData post)
        {
            var ack = new Acknowledgement();
            var context = DbContext;
            var gr = await context.ChatGroups.Where(i => i.Id == post.GroupId)
                                             .Include(i => i.UserLastReadMessages)
                                             .FirstOrDefaultAsync();
            if (gr == null)
            {
                ack.IsSuccess = false;
                ack.ErrorMessage = new List<string>() { "Chat group is not found" };
            }

            var cm = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                IsSystem = false,
                GroupId = post.GroupId,
                ChatTextMessage = post.Text != string.Empty ?
                new ChatTextMessage()
                {
                    Text = post.Text
                } : null,
                CreatedBy = post.CurrentUserId,
                CreatedDate = DateTime.Now,
            };
            gr.LastMessageId = cm.Id;
            gr.LastChatMessage = cm;
            var ulrm = gr.UserLastReadMessages.Where(i => i.UserId == post.CurrentUserId).FirstOrDefault();
            if (ulrm != null)
            {
                ulrm.Time = DateTime.Now;
            }
            else
            {
                gr.UserLastReadMessages.Add(new UserLastReadMessage()
                {
                    ChatGroupId = post.GroupId,
                    UserId = post.CurrentUserId,
                    Time = DateTime.Now
                });
            }

            await ack.TrySaveChangesAsync(context);
            if (ack.IsSuccess == true)
            {
                //Call SignalR
            }
            return ack;
        }

        /// <summary>
        /// Get list of user
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<List<UserViewModel>>> GetUserList(string searchValue)
        {
            var ack = new Acknowledgement<List<UserViewModel>>();
            var context = DbContext;

            var predicate = PredicateBuilder.New<User>(false);

            if (string.IsNullOrEmpty(searchValue))
            {
                var s = searchValue.Trim().ToLower();
                predicate = predicate.And(i => i.DisplayName.ToLower().Contains(s));
            }

            var users = await context.Users.Where(predicate).ToListAsync();
            var result = users.Select(i =>
            {
                var u = new UserViewModel();
                u.MapDTOUser(i);
                return u;
            }).Take(50).ToList();

            ack.IsSuccess = true;
            ack.Data = result;
            return ack;
        }

        /// <summary>
        /// Create new chat group
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Acknowledgement> CreateChatGroup(CreateChatGroupModel post)
        {
            var ack = new Acknowledgement();
            var context = DbContext;

            var message = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                ChatTextMessage = new ChatTextMessage()
                {
                    Text = post.ChatMessageData.Text
                }
            };
            var chatGroup = new ChatGroup()
            {
                Id = Guid.NewGuid(),
                CreatedBy = CurrentUserId(),
                Name = StringHelper.CreateChatGroupName(post.Users),
                CreatedDate = DateTime.Now,
                LastChatMessage = message,
                Status = (int)EChatGroupStatus.InGroup,
                UserLastReadMessages = new List<UserLastReadMessage>() {
                    new UserLastReadMessage()
                    {
                        LastReadMessageId = message.Id,
                        Time = DateTime.Now,
                        UserId = CurrentUserId(),
                    }
                },
                ChatMembers = post.Users.Select(i => new ChatMember()
                {
                    AddedBy = CurrentUserId(),
                    AddedDate = DateTime.Now,
                    UserId = i.Id
                }).ToList(),
                ChatMessages = new List<ChatMessage>() { message }
            };

            context.ChatGroups.Add(chatGroup);
            await ack.TrySaveChangesAsync(context);
            return ack;
        }
        #endregion
        #region Others
        public ChatMessageViewModel MapChatMessageViewModel(ChatMessage m, User u)
        {
            var cm = new ChatMessageViewModel();
            cm.MapDTOChatMessage(m);
            cm.CreatedByName = u.DisplayName;
            return cm;
        }
        public ChatMemberViewModel MapChatMemberViewModel(User m, User addBy)
        {
            var cm = new ChatMemberViewModel()
            {
                UserId = m.Id,
                DisplayName = m.DisplayName,
                IsOnline = GetCurrentStatusOnlineByUserId(m.Id),
                AddByName = addBy.DisplayName
            };
            return cm;
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
        #endregion
    }

}
