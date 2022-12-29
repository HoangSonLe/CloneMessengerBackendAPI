using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Models;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using CloneMessengerBackendAPI.Service.Serviecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CloneMessengerBackendAPI.Web.API
{
    public class ChatController : BaseAPIController
    {
        public ChatController(IMessageServices messageServices) : base(messageServices)
        {
        }


        [HttpGet]
        // GET: ChatGroup
        public async Task<Acknowledgement<List<ChatGroupViewModel>>> GetChatGroupList(PaginationModel post)
        {
            var p = post == null ?new PaginationModel() : post;
            var result = await MessageServices.GetChatGroups(p);
            return result;
        }
        [HttpPost]
        public async Task<Acknowledgement<ChatGroupDetailViewModel>> GetChatGroupDetail(ChatMessagePaginationModel post)
        {
            var result = await MessageServices.GetChatGroupDetail(post);
            return result;
        } 
        [HttpPost]
        public async Task<Acknowledgement<List<ChatMessageViewModel>>> GetMessageList(ChatMessagePaginationModel post)
        {
            var result = await MessageServices.GetMessageList(post);
            return result;
        }
        [HttpPost]
        public async Task<Acknowledgement> SendMessage(ChatMessagePostData post)
        {
            var result = await MessageServices.SendMessage(post);
            return result;
        } 
        [HttpPost]
        public async Task<Acknowledgement<List<UserViewModel>>> GetUserList(string searchValue)
        {
            var result = await MessageServices.GetUserList(searchValue);
            return result;
        }
        [HttpPost]
        public async Task<Acknowledgement> CreateChatGroup(CreateChatGroupModel post)
        {
            var result = await MessageServices.CreateChatGroup(post);
            return result;
        }
    }
}
