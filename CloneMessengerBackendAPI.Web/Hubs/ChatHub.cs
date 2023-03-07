using BasicChat;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.SignalRModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace CloneMessengerBackendAPI.Web.Hubs
{
    public interface IChatHub
    {
        void test(MessageSignalRModel model);
        Task sendMessage(MessageSignalRModel model);
        Task sendMessageWithCreateConversation(CreateConversationModel model);
        Task updateMessageInfo(MessageInforModel model);
        Task updateStatusReadMessage(MessageStatus model);
    }
    public class ChatHub : Hub<IChatHub>,IChatHubService
    {
        private static ChatHub hub;
        //public static ChatHub Default
        //{
        //    get
        //    {
        //        if (hub == null)
        //        {
        //            hub = new ChatHub();
        //        }
        //        return hub;
        //    }
        //}
        private readonly static ConnectionMapping<string> _connections =
                   new ConnectionMapping<string>();
        protected IHubConnectionContext<IChatHub> ChatHubContext
        {
            get
            {
                var x = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub, IChatHub>();
                return x.Clients;
            }
        }
        public override Task OnConnected()
        {
            test();
            try
            {
                string userId = GetUserIdClaim();
                _connections.Add(userId, Context.ConnectionId);
            }
            catch (Exception ex)
            {

            }
            
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            string userId = GetUserIdClaim();

            if (!_connections.GetConnections(userId).Contains(Context.ConnectionId))
            {
                _connections.Add(userId, Context.ConnectionId);
            }
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            string userId = GetUserIdClaim();
            _connections.Remove(userId, Context.ConnectionId);
            UpdateOnline(userId);
            return base.OnDisconnected(stopCalled);
        }
        #region PRIVATE

        private List<string> GetConnectionIdsByUserIds(List<Guid> userIds)
        {
            var connectionIds = _connections.GetConnectionsByKeys(userIds.Select(i=>i.ToString()).ToList()).ToList();
            return connectionIds;
        }  
        private string GetUserIdClaim()
        {
            var userId = string.Empty;
            var tmp = Context.Request.QueryString["token"];
            var current = Authentication.ParseToken(tmp);
            var identity = (ClaimsPrincipal)current;
            if(identity != null)
            {
                var result = (new UserModel()).ParseClaim(identity);
                userId = result.Id.ToString();
                if (String.IsNullOrEmpty(userId))
                {
                    throw new Exception("UserId can not found");
                }
            }
            return userId;
        }
        #endregion
        #region PUBLIC
        private void UpdateOnline(string userId)
        {

        }
        public void test()
        {
            Clients.All.test(new MessageSignalRModel() { });
        }
        public List<Guid> GetUserOnlines(List<Guid> userIds)
        {
            var connectionIds = _connections.GetUserOnlines(userIds.Select(i => i.ToString()).ToList()).ToList();
            return connectionIds;
        }
        /// <summary>
        /// Update status READ
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public async Task UpdateStatusReadMessage(MessageStatus model, List<Guid> userIds)
        {   
            var conIds = GetConnectionIdsByUserIds(userIds);
            await ChatHubContext.Clients(conIds).updateStatusReadMessage(model);
        } 
        /// <summary>
        /// Update status Pending, Sent
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public async Task UpdateMessageInfo(MessageInforModel model, List<Guid> userIds)
        {   
            var conIds = GetConnectionIdsByUserIds(userIds);
            await ChatHubContext.Clients(conIds).updateMessageInfo(model);
        }
        public async Task SendMessage(MessageSignalRModel model, List<Guid> userIds)
        {
            var conIds = GetConnectionIdsByUserIds(userIds);
            ChatHubContext.All.test(new MessageSignalRModel() { });
            await ChatHubContext.Clients(conIds).sendMessage(model);
        }
        public async Task SendMessageWithCreateConversation(CreateConversationModel model, List<Guid> userIds)
        {
            var conIds = GetConnectionIdsByUserIds(userIds);
            await ChatHubContext.Clients(conIds).sendMessageWithCreateConversation(model);
        }
        #endregion
    }

}