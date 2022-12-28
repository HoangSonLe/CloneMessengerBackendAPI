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
