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
        public async Task<Acknowledgement<List<ChatGroupViewModel>>> GetAllChatGroup()
        {
            var result = await MessageServices.GetChatGroups(new ChatMessageFilterModel());
            return result;
        }
        [HttpPost]
        public async Task<Acknowledgement<ChatGroupViewModel>> GetChatMessageDetail(Guid chatGroupId)
        {
            var result = await MessageServices.GetChatMessageDetail(chatGroupId);
            return result;
        }
    }
}
