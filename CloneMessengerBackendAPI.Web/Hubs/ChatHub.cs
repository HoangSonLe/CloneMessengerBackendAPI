using CloneMessengerBackendAPI.Service.Models.BaseModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Messaging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace CloneMessengerBackendAPI.Web.Hubs
{
    public interface IChat
    {
        //Task<IChat> SendMessage();
        void test(string name, string message);
    }

    public class ChatHub : Hub<IChat>
    {
        private readonly IDictionary<string, string> _connections;
        public ChatHub() : base()
        {
            _connections = new Dictionary<string, string>();
        }
        public override Task OnConnected()
        {
            CreateNewConnection();
            test();
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            CreateNewConnection();
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out string userId))
            {
                _connections.Remove(Context.ConnectionId);
                UpdateOnline(userId);
            }

            return base.OnDisconnected(stopCalled);
        }

        private List<string> GetConnectionIds()
        {
            var connectionIds = _connections.Select(i => i.Key).ToList();
            return connectionIds;
        }
        private List<Guid> GetUserConnected()
        {
            var userIds = _connections.Select(i => Guid.Parse(i.Value)).ToList();
            return userIds;
        }





        private void UpdateOnline(string userId)
        {

        }
        public void test()
        {
            Clients.All.test("Huy", "Connected");
        }
        private void CreateNewConnection()
        {
            var userId = string.Empty;

            if (Context.User.Identity.IsAuthenticated == true)
            {
                userId = ((ClaimsIdentity)Context.User.Identity).Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            else
            {
                var jwtKey = Context.QueryString["jwt"];
                if (string.IsNullOrEmpty(jwtKey) == false)
                {
                    var currentClaim = Authentication.ParseToken(jwtKey);
                    userId = currentClaim.Claims.Where(i=>i.Type == ClaimTypes.NameIdentifier).First().Value;
                }
            }

            if (String.IsNullOrEmpty(userId))
            {
                userId = (Guid.NewGuid()).ToString();
            }
            var connectionId = Context.ConnectionId;
            _connections.Add(connectionId, userId);
        }

       


    }
}