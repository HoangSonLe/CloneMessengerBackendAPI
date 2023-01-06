using BasicChat;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.SignalRModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace CloneMessengerBackendAPI.Web.Hubs
{
    public interface IChatHub
    {
        void test(string name, string message);
        Task sendMessage(MessageSignalRModel model);
        void updateStatusMessage(MessageSignalRWithStatus model);
    }
    public class ChatHub : Hub<IChatHub>,IChatHubService
    {
        private static ChatHub hub;
        public static ChatHub Default
        {
            get
            {
                if (hub == null)
                {
                    hub = new ChatHub();
                }
                return hub;
            }
        }
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
            string userId = GetUserIdClaim();
            _connections.Add(userId, Context.ConnectionId);
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
            var result = (new UserModel()).ParseClaim(identity);
            userId = result.Id.ToString();
            if (String.IsNullOrEmpty(userId))
            {
                throw new Exception("UserId can not found");
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
            Clients.All.test("Hoang Son","Connected");
        }
        public void UpdateStatusMessage(MessageSignalRWithStatus model, List<Guid> userIds)
        {
            var conIds = GetConnectionIdsByUserIds(userIds);
            ChatHubContext.Clients(conIds).updateStatusMessage(model);
        }
        public async Task SendMessage(MessageSignalRModel model, List<Guid> userIds)
        {
            var conIds = GetConnectionIdsByUserIds(userIds);
            await ChatHubContext.Clients(conIds).sendMessage(model);
        }
        #endregion
    }
}