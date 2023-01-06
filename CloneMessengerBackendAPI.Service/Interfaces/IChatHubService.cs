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
        void UpdateStatusMessage(MessageSignalRWithStatus model, List<Guid> userIds);
    }
}
