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
using System.Runtime.Caching;
using CloneMessengerBackendAPI.Model.ConfigureModel;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.SignalRModels;
using System.Xml.Linq;

namespace CloneMessengerBackendAPI.Service.Serviecs
{
    public class MessageServices : BaseService, IMessageService
    {
        public MessageServices(IChatHubService hub) : base(hub)
        {
        }
        #region API
        /// <summary>
        /// Get list of chat group
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<PaginationModel<List<ChatGroupViewModel>>>> GetChatGroups(PaginationModel post)
        {
            var currentUserId = post.CurrentUser.Id;
            var context = DbContext;

            var seftGroupIds = context.ChatMembers.Where(i => i.UserId == currentUserId).Select(i => i.ChatGroupId);
            var queryGroup = context.ChatGroups.Where(i => seftGroupIds.Contains(i.Id));
            var query = (from gr in queryGroup
                             //join lm in context.UserLastReadMessages.Where(i => i.UserId == currentUserId)
                             //on gr.Id equals lm.ChatGroupId into lm1
                             //from lm in lm1.DefaultIfEmpty()
                         select new
                         {
                             ChatGroup = gr,
                             LastMessage = gr.LastChatMessage,
                             //LastReadMessage = lm,
                         }).OrderByDescending(i => i.LastMessage != null ? i.LastMessage.CreatedDate : i.ChatGroup.CreatedDate).AsQueryable();

            if (post.PageSize.HasValue)
            {
                query = query.Skip(post.Skip).Take(post.PageSize.Value);
            }
            var groupData = await query.ToListAsync();

            var users = await (from g in queryGroup
                               join cm in context.ChatMembers on g.Id equals cm.ChatGroupId
                               select new { cm, cm.User, cm.AddedUser }).ToListAsync();

            var userLastReadMessages = await context.UserLastReadMessages.Where(i => i.UserId == currentUserId && seftGroupIds.Contains(i.ChatGroupId)).ToListAsync();
            var join = (from g in groupData
                        join l in userLastReadMessages on g.ChatGroup.Id equals l.ChatGroupId into l1
                        from l in l1.DefaultIfEmpty()
                        select new
                        {
                            Group = g,
                            LastReadMessage = l
                        }).ToList();
            var listChatGroup = new List<ChatGroupViewModel>();

            foreach (var item in join)
            {
                var g = item.Group;
                var gr = g.ChatGroup;
                var lm = g.LastMessage;
                var group = new ChatGroupViewModel();
                group.MapDTOChatGroup(gr, lm, ChatHub);
                if (lm != null)
                {
                    group.MessageStatus = await GetMessageStatus(g.ChatGroup.Id);
                }
                listChatGroup.Add(group);
            }
            var nextSkip = post.Skip + listChatGroup.Count();
            var ack = new Acknowledgement<PaginationModel<List<ChatGroupViewModel>>>()
            {
                IsSuccess = true,
                Data = new PaginationModel<List<ChatGroupViewModel>>()
                {
                    HasMore = ((await queryGroup.CountAsync()) > nextSkip),
                    Data = listChatGroup,
                    Skip = nextSkip
                },
            };
            return ack;
        }
        /// <summary>
        /// Update last read message of user
        /// </summary>
        /// <param name="chatGroupId"></param>
        /// <returns></returns>
        public async Task<Acknowledgement> ReadLastMessage(Guid chatGroupId, Guid currentUserId)
        {
            var ack = new Acknowledgement();
            var context = DbContext;
            var queryData = await (from g in context.ChatGroups.Where(i => i.Id == chatGroupId)
                                   join cm in context.ChatMessages on g.LastMessageId equals cm.Id into cm1
                                   from cm in cm1.DefaultIfEmpty()
                                   select new
                                   {
                                       ChatMessage = cm,
                                       ChatMembers = g.ChatMembers.Where(i => i.IsRemoved == false)
                                   }).FirstOrDefaultAsync();
            var lastMessage = queryData.ChatMessage;
            if (lastMessage != null)
            {
                var isNeedSave = true;
                var dbData = await context.UserLastReadMessages.FirstOrDefaultAsync(i => i.UserId == currentUserId && i.ChatGroupId == chatGroupId);
                if (dbData == null)
                {
                    //thêm mới
                    var userLastRead = new UserLastReadMessage()
                    {
                        ChatGroupId = chatGroupId,
                        LastReadMessageId = lastMessage.Id,
                        Time = DateTime.Now,
                        UserId = currentUserId
                    };
                    context.UserLastReadMessages.Add(userLastRead);
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
                var tmp = await GetMessageStatus(lastMessage.GroupId);
                var memberIds = queryData.ChatMembers.Select(i => i.UserId).ToList();
                //Call SignalR with status message
                await ChatHub.UpdateStatusReadMessage(tmp, memberIds);
                if (ack.IsSuccess == false)
                {
                    return ack;
                }
            }
            else
            {
                ack.IsSuccess = true;
            }

            return ack;
        }
        /// <summary>
        /// Get list messages in group -- has pagination
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<PaginationModel<List<ChatMessageGroupByTimeViewModel>>>> GetMessageList(ChatMessagePaginationModel post)
        {
            var context = DbContext;
            var ack = new Acknowledgement<PaginationModel<List<ChatMessageGroupByTimeViewModel>>>();
            var messageQuery = context.ChatMessages.Where(i => i.GroupId == post.ChatGroupId)
                                               .Include(i => i.ChatTextMessage)
                                               .Include(i => i.User).OrderByDescending(i => i.CreatedDate).AsQueryable();
            var totalMessages = messageQuery.Count();
            if (post.PageSize.HasValue)
            {
                messageQuery = messageQuery.Skip(post.Skip).Take(post.PageSize.Value);
            }
            var messages = await messageQuery.ToListAsync();
            //Neu group thay doi thu tu list thi bo order o query
            var groupByTime = messages.GroupBy(i => i.ContinuityKeyByTime).ToList();
            var result = new List<ChatMessageGroupByTimeViewModel>();
            groupByTime.ForEach(i =>
            {
                var groupUser = i.GroupBy(j => j.ContinuityKeyByUser).ToList();
                var groupTime = new ChatMessageGroupByTimeViewModel()
                {
                    ContinuityKeyByTime = i.Key,
                    GroupMessageTime = i.OrderBy(j => j.CreatedBy).First().CreatedDate,
                    GroupMessageListByUser = groupUser.Select(k => new ChatMessageGroupByUserViewModel()
                    {
                        ContinuityKeyByUser = k.Key,
                        IsMyMessage = k.First().CreatedBy == post.CurrentUser.Id,
                        Messages = k.Select(j => MapChatMessageViewModel(j, new UserViewModel()
                        {
                            Id = j.User.Id,
                            DisplayName = j.User.DisplayName
                        })).OrderBy(j => j.CreatedDate).ToList()
                    }).OrderBy(j => j.Messages.First().CreatedDate).ToList()
                };
                result.Add(groupTime);
            });
            var nextSkip = post.Skip + messages.Count();
            ack.Data = new PaginationModel<List<ChatMessageGroupByTimeViewModel>>()
            {
                Data = result.OrderBy(i => i.GroupMessageTime).ToList(),
                HasMore = totalMessages > nextSkip,
                Skip = nextSkip,
            };
            ack.IsSuccess = true;
            return ack;

        }
        public async Task<MessageStatus> GetMessageStatus(Guid chatGroupId)
        {
            var context = DbContext;
            var lmList = await context.UserLastReadMessages.Where(i => i.ChatGroupId == chatGroupId)
                                                           .Include(i => i.ChatMessage)
                                                           .Include(i => i.User)
                                                           .ToListAsync();
            var l = lmList.Select(i => new MessageStatusItem()
            {
                ChatMessageId = i.LastReadMessageId,
                ReadTime = i.Time,
                LastMessageCreatedDate = i.ChatMessage.CreatedDate,
                UserId = i.UserId,
                UserName = i.User.DisplayName,
            }).ToList();
            var result = new MessageStatus()
            {
                ChatGroupId = chatGroupId,
                MessageStatusItemList = l,
            };
            return result;
        }
        /// <summary>
        /// Get chat message detail (list messages, list members,...)
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<ChatGroupDetailViewModel>> GetChatGroupDetail(ChatMessagePaginationModel post)
        {
            var ack = new Acknowledgement<ChatGroupDetailViewModel>();
            var context = DbContext;
            //Read last message before load message detail
            var readLastAck = await ReadLastMessage(post.ChatGroupId, post.CurrentUser.Id);
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
                                             //.Include(i => i.UserLastReadMessages)
                                             .FirstOrDefaultAsync();



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
                ListMembers = DTOMapper.MapChatMemberViewModelList(queryG.ChatMembers.ToList(), ChatHub),
                GroupMessageListByTime = messagesAck.Data,
                IsRemoved = queryG.ChatMembers.Any(i => i.UserId == post.CurrentUser.Id && i.IsRemoved == true),
                DefaultChatMessage = new ChatMessagePostData()
                {
                    GroupId = queryG.Id,
                    Text = string.Empty,
                },
            };
            result.MessageStatus = await GetMessageStatus(queryG.Id);


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
            //User post message => signalR message with status SENDING to user
            //Service save message sucess => signalR message with status SENT to all user
            var ack = new Acknowledgement();
            var context = DbContext;
            var cacheGroup = CacheMessage.GetOrCreateCacheGroup(context, post.GroupId, post.CurrentUser.Id);
            var gr = await context.ChatGroups.Where(i => i.Id == post.GroupId)
                                             .Include(i => i.ChatMembers)
                                             //.Include(i => i.UserLastReadMessages)
                                             .FirstOrDefaultAsync();
            if (gr == null && post.GroupId != Guid.Empty)
            {
                ack.IsSuccess = false;
                ack.ErrorMessage = new List<string>() { "Chat group is not found" };
                return ack;
            }
            var currentUserId = post.CurrentUser.Id;
            var cm = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                IsSystem = false,
                GroupId = post.GroupId,
                ContinuityKeyByTime = cacheGroup.KeyGroupByTime,
                ContinuityKeyByUser = cacheGroup.KeyGroupByUserId,
                ChatTextMessage = post.Text != string.Empty ?
                new ChatTextMessage()
                {
                    Text = post.Text
                } : null,
                CreatedBy = currentUserId,
                CreatedDate = DateTime.Now,
            };
            gr.LastMessageId = cm.Id;
            gr.LastChatMessage = cm;
            //Cập nhật lại userlastread
            var ulrm = gr.UserLastReadMessages.Where(i => i.UserId == currentUserId).FirstOrDefault();
            if (ulrm != null)
            {
                ulrm.Time = DateTime.Now;
                ulrm.LastReadMessageId = cm.Id;
            }
            else
            {
                gr.UserLastReadMessages.Add(new UserLastReadMessage()
                {
                    ChatGroupId = post.GroupId,
                    UserId = currentUserId,
                    Time = DateTime.Now
                });
            }
            //Call SignalR SendMessage
            var mes = new ChatMessageGroupByUserViewModel()
            {
                ContinuityKeyByUser = cm.ContinuityKeyByUser,
                IsMyMessage = cm.CreatedBy == currentUserId,
                Messages = new List<ChatMessageViewModel>()
                {
                    MapChatMessageViewModel(cm,new UserViewModel(post.CurrentUser),EMessageStatus.Sending)
                },
            };
            var signalRModel = new MessageSignalRModel()
            {
                ChatGroupId = cm.GroupId,
                IsNewGroupByTime = cacheGroup.IsNewGroupByTime,
                IsNewGroupByUser = cacheGroup.IsNewGroupByUser,
                MessageGroupByTime = new ChatMessageGroupByTimeViewModel()
                {
                    ContinuityKeyByTime = cm.ContinuityKeyByTime,
                    GroupMessageTime = cm.CreatedDate,
                    GroupMessageListByUser = new List<ChatMessageGroupByUserViewModel>()
                    {
                       mes
                    },
                },
                MessageGroupByUser = mes
            };

            //Call SignalR with stauts Sending currentUser
            await ChatHub.SendMessage(signalRModel, new List<Guid>() { currentUserId });
            await ack.TrySaveChangesAsync(context);
            if (ack.IsSuccess == true)
            {
                var memberIds = gr.ChatMembers.Where(j => j.IsRemoved == false && j.UserId != currentUserId).Select(i => i.UserId).ToList();
                mes.IsMyMessage = false;
                mes.Messages.ForEach(i =>
                {
                    i.MessageStatus = EMessageStatus.Sent;
                });
                //Call SignalR with stauts Sending otherMembers
                await ChatHub.SendMessage(signalRModel, memberIds);
                var info = new MessageInforModel()
                {
                    ChatGroupId = gr.Id,
                    KeyGroupByTime = cm.ContinuityKeyByTime,
                    KeyGroupByUser = cm.ContinuityKeyByUser,
                    MessageId = cm.Id,
                    Status = EMessageStatus.Sent
                };
                //Call signalR mark read
                await ReadLastMessage(gr.Id, currentUserId);
                //Call SignalR update status message for sender
                await ChatHub.UpdateMessageInfo(info, new List<Guid>() { currentUserId });
            }
            return ack;
        }

        /// <summary>
        /// Get list of user
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<List<UserViewModel>>> GetUserList(string searchValue, Guid currentUserId)
        {
            var ack = new Acknowledgement<List<UserViewModel>>();
            var context = DbContext;

            var predicate = PredicateBuilder.New<User>(false);

            if (string.IsNullOrEmpty(searchValue))
            {
                ack.IsSuccess = true;
                ack.Data = new List<UserViewModel>();
                return ack;
            }

            var s = searchValue.Trim().ToLower();
            predicate = predicate.And(i => i.DisplayName.ToLower().Contains(s));
            predicate = predicate.And(i => i.Id != currentUserId);
            var users = await context.Users.Where(predicate).ToListAsync();
            var onlines = ChatHub.GetUserOnlines(users.Select(i => i.Id).ToList());
            var result = users.Select(i =>
            {
                var u = new UserViewModel();
                u.MapDTOUser(i);

                u.IsActive = onlines.Contains(i.Id);
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
            var newChatGroupId = Guid.NewGuid();
            var cacheGroup = CacheMessage.GetOrCreateCacheGroup(context, newChatGroupId, post.CurrentUser.Id);
            
            var currentUserId = post.CurrentUser.Id;
            var userIds = post.UserIds;
            userIds.Add(currentUserId);
            var users = context.Users.Where(i => userIds.Contains(i.Id)).ToList();
            var message = new ChatMessage()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                GroupId = newChatGroupId,
                IsSystem = false,
                ContinuityKeyByTime = cacheGroup.KeyGroupByTime,
                ContinuityKeyByUser = cacheGroup.KeyGroupByUserId,
                ChatTextMessage = post.Text != string.Empty ?
                new ChatTextMessage()
                {
                    Text = post.Text
                } : null,
                CreatedBy = currentUserId,
            };
            var chatGroup = new ChatGroup()
            {
                Id = newChatGroupId,
                CreatedBy = currentUserId,
                Name = StringHelper.CreateChatGroupName(users, currentUserId),
                UserIds = StringHelper.CastMemberToUserIdForChat(users.Select(i => i.Id).ToList()),
                CreatedDate = DateTime.Now,
                ChatMembers = users.Select(i => new ChatMember()
                {
                    AddedBy = currentUserId,
                    AddedDate = DateTime.Now,
                    UserId = i.Id
                }).ToList(),
                //ChatMessages = new List<ChatMessage>() { message }
            };

            context.ChatGroups.Add(chatGroup);
            await ack.TrySaveChangesAsync(context);
            chatGroup.LastChatMessage = message;
            chatGroup.UserLastReadMessages = new List<UserLastReadMessage>() {
                    new UserLastReadMessage()
                    {
                        LastReadMessageId = message.Id,
                        Time = DateTime.Now,
                        UserId = currentUserId,
                    }
                };
            await ack.TrySaveChangesAsync(context);
            if (ack.IsSuccess)
            {
                var g = new ChatGroupViewModel();
                g.MapDTOChatGroup(chatGroup, message, ChatHub);
                g.MessageStatus = await GetMessageStatus(g.Id);

                //Send other users in group except currentUser
                await ChatHub.SendMessageWithCreateConversation(new CreateConversationModel()
                {
                    IsCreateUser = false,
                    Group = g
                }, userIds.Where(i => i != currentUserId).ToList());

                //Get detail chat group
                var detail = (await GetChatGroupDetail(new ChatMessagePaginationModel()
                {
                    ChatGroupId = g.Id,
                    CurrentUser = new UserModel()
                    {
                        Id = currentUserId
                    }
                })).Data;
                //Send other users in group except currentUser
                await ChatHub.SendMessageWithCreateConversation(new CreateConversationModel()
                {
                    IsCreateUser = true,
                    Group = g,
                    Conversation = detail
                }, currentUserId.ToSingleList());
            }
            return ack;
        }

        /// <summary>
        /// Search group same members
        /// </summary>
        /// <param name="memberIds"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public async Task<Acknowledgement<ChatGroupDetailViewModel>> SearchChatGroup(List<Guid> memberIds, UserModel currentUser)
        {
            var ack = new Acknowledgement<ChatGroupDetailViewModel>();
            var context = DbContext;
            var users = context.Users.Where(i => memberIds.Contains(i.Id)).ToList();
            memberIds.Add(currentUser.Id);
            var s = StringHelper.CastMemberToUserIdForChat(memberIds);
            var group = context.ChatGroups.FirstOrDefault(i => i.UserIds == s);
            
            if (group != null)
            {
                ack = await GetChatGroupDetail(new ChatMessagePaginationModel()
                {
                    ChatGroupId = group.Id,
                    CurrentUser = currentUser
                });
                ack.Data.IsTmp = true;
            }
            else
            {
              
                ack = new Acknowledgement<ChatGroupDetailViewModel>()
                {
                    IsSuccess = true,
                    Data = new ChatGroupDetailViewModel()
                    {
                        IsTmp = true,
                        ListMembers = users.Select(i => new ChatMemberViewModel()
                        {
                            UserId = i.Id,
                            DisplayName = i.DisplayName
                        }).ToList()
                    }
                };
            }
            var name = StringHelper.CreateChatGroupName(users, currentUser.Id);
            var cName = name.Count() > 0 ? "to" : "";
            ack.Data.Name = $"New message {cName} {name}";
            return ack;
        }
        #endregion
        #region Others
        public ChatMessageViewModel MapChatMessageViewModel(ChatMessage m, UserViewModel u, EMessageStatus status = EMessageStatus.Sent)
        {
            var cm = new ChatMessageViewModel();
            cm.MapDTOChatMessage(m);
            cm.CreatedByName = u.DisplayName;
            cm.MessageStatus = status;
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
