using CloneMessengerBackendAPI.Service.Serviecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Interfaces
{
    public interface IBasicService : IDisposable
    {

    }
    public interface IServiceLocator
    {
        IMessageService MessageService { get; }
        IUserService UserService { get; }
    }
    public class IoCServiceLocator : IServiceLocator
    {
        public IMessageService MessageService => new MessageServices();

        public IUserService UserService => new UserServices();
    }
}
