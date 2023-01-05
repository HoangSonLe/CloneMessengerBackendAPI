using CloneMessengerBackendAPI.Model.Model;
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

        public bool GetCurrentStatusOnlineByUserId(Guid userId)
        {
            return true;
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
