using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Interfaces
{
    public interface IUserService : IBasicService
    {
        Task<Acknowledgement<LoginResultModel>> Login(LoginModel post);
        Task<Acknowledgement<LoginResultModel>> Register(RegisterModel post);
        Task<Acknowledgement<List<UserViewModel>>> GetOnlineUserList(Guid currentUserId);
    }
}
