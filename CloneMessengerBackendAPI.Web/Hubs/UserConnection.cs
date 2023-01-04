using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloneMessengerBackendAPI.Web.Hubs
{
    public class UserConnection
    {
        public Guid UserId { get; set; }
        public Guid ConnectionId { get; set; }
    }

}