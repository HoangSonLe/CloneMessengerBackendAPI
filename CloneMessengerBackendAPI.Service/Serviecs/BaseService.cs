using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Serviecs
{
    public abstract class BaseService : IDisposable
    {
        private readonly IChatHubService _hub;
        public IChatHubService ChatHub => _hub;

        public BaseService(IChatHubService hub)
        {
            _hub = hub;
        }
        private CloneMessengerDbContext _DbContext;
        public CloneMessengerDbContext DbContext
        {
            get
            {
                if(_DbContext == null)
                {
                    _DbContext = new CloneMessengerDbContext();
                }
                return _DbContext;
            }
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
        ~BaseService()
        {
            this.Dispose();
        }
    }
}
