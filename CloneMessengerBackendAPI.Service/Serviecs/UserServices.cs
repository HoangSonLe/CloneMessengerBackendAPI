using CloneMessengerBackendAPI.Service.Helper;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Serviecs
{
    public class UserServices: BaseService,IUserService
    {
        public UserServices(IChatHubService hub) : base(hub)
        {
        }

        public async Task<Acknowledgement<LoginResultModel>> Login(LoginModel post)
        {
            var context = DbContext;
            var ack = new Acknowledgement<LoginResultModel>();
            if(string.IsNullOrEmpty(post.UserName) || string.IsNullOrEmpty(post.Password))
            {
                ack.AddMessages("Please fill username and password!");
                return ack;
            }
            var userName = post.UserName.Trim().ToLower();
            var md5Pass = StringHelper.ToMD5Byte(post.Password);
            var user = await context.Users.Where(i=>i.UserName.ToLower() ==userName && i.MD5Password == md5Pass).FirstOrDefaultAsync();

            if (user == null)
            {
                ack.AddMessages("User is not found!");
                return ack;
            }
            var token = Authentication.GenerateJWT(new UserModel()
            {
                Id = user.Id,
                Username = user.UserName,
                DisplayName = user.DisplayName,
                Password = post.Password
            });

            ack.IsSuccess = true;
            ack.Data = new LoginResultModel()
            {
                Token = token,
                UserId = user.Id,
            };
            return ack;
        }
    }
}
