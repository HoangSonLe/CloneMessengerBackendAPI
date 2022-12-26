using CloneMessengerBackendAPI.Model.Model;
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
        public async Task<List<ChatGroup>> GetAllChatGroup()
        {
            var result = await MessageServices.GetChatGroups();
            return result;
        }
    }
}
