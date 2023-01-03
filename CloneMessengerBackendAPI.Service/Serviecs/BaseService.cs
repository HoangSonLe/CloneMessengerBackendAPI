using CloneMessengerBackendAPI.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Serviecs
{
    public abstract class BaseService : IDisposable
    {
        public CloneMessengerDbContext DbContext
        {
            get { return new CloneMessengerDbContext(); }
        }

        public Guid CurrentUserId()
        {
            return Guid.Parse("29CA1C9B-04AF-45CE-A5D9-DC7849A35EBC");
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
