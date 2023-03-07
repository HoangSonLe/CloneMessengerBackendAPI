using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using CloneMessengerBackendAPI.Service.Serviecs;
using CloneMessengerBackendAPI.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CloneMessengerBackendAPI.Web.API
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class ChatController : BaseAPIController
    {
        public ChatController(IUserService userService, IMessageService messageService) : base(userService, messageService)
        {
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetPageDefaultModel()
        {
            var result = new Acknowledgement<PageDefaultModel>()
            {
                IsSuccess = true,
                Data = new PageDefaultModel()
                {
                    ChatGroupDetailViewModel = new ChatGroupDetailViewModel()
                    {
                        IsTmp = true,
                    },
                    ChatGroupViewModel = new ChatGroupViewModel()
                    {
                        IsTmp = true,
                        Name = "New message"
                    }
                }
            };
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        // GET: ChatGroup
        public async Task<IHttpActionResult> GetChatGroupList(PaginationModel post)
        {
            var p = post == null ? new PaginationModel() : post;
            p.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.GetChatGroups(p);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> ReadLastMessage(Guid chatGroupId)
        {
            var result = await MessageServices.ReadLastMessage(chatGroupId,GetCurrentUserModel().Id);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetChatGroupDetail(ChatMessagePaginationModel post)
        {
            post.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.GetChatGroupDetail(post);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetMessageList(ChatMessagePaginationModel post)
        {
            post.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.GetMessageList(post);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> SendMessage(ChatMessagePostData post)
        {
            post.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.SendMessage(post);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> GetUserList(string searchValue)
        {
            var result = await MessageServices.GetUserList(searchValue, GetCurrentUserModel().Id);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> CreateChatGroup(CreateChatGroupModel post)
        {
            post.CurrentUser = GetCurrentUserModel();
            var result = await MessageServices.CreateChatGroup(post);
            return MapToIHttpActionResult(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> SearchChatGroup(List<Guid> memberIds)
        {
            var result = await MessageServices.SearchChatGroup(memberIds, GetCurrentUserModel());
            return MapToIHttpActionResult(result);
        }
    }
}
