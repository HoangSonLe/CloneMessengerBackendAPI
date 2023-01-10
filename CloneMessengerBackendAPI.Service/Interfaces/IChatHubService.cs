using CloneMessengerBackendAPI.Service.Models.SignalRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Interfaces
{
    public interface IChatHubService
    {
        void test();
        Task SendMessage(MessageSignalRModel model, List<Guid> userIds);
        Task UpdateMessageInfo(MessageInforModel model, List<Guid> userIds);
        Task UpdateStatusReadMessage(MessageStatus model, List<Guid> userIds);
    }
}
