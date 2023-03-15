using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Helper;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using LinqKit;
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
                DisplayName = user.DisplayName,
                UserId = user.Id,
                AvatarFileId = user.AvatarFileId 
            };
            return ack;
        }
        public async Task<Acknowledgement<LoginResultModel>> Register(RegisterModel post)
        {
            var context = DbContext;

            var ack = new Acknowledgement<LoginResultModel>();
            if (string.IsNullOrEmpty(post.DisplayName) || string.IsNullOrEmpty(post.UserName) || string.IsNullOrEmpty(post.Password))
            {
                ack.AddMessages("Please fill displayname, username and password!");
                return ack;
            }
            var userName = post.UserName.Trim().ToLower();
            var md5Pass = StringHelper.ToMD5Byte(post.Password);
            var user = await context.Users.Where(i => i.UserName.ToLower() == userName && i.MD5Password == md5Pass).FirstOrDefaultAsync();

            if (user != null)
            {
                ack.AddMessages("User is exist!");
                return ack;
            }
            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                DisplayName = post.DisplayName,
                MD5Password = md5Pass,
                CreatedDate = DateTime.Now,
                AvatarFileId = post.AvatarFileId,
            };
            context.Users.Add(newUser);
            await ack.TrySaveChangesAsync(context);
            if(ack.IsSuccess == false)
            {
                return ack;
            }
            var token = Authentication.GenerateJWT(new UserModel()
            {
                Id = newUser.Id,
                Username = newUser.UserName,
                DisplayName = newUser.DisplayName,
                Password = post.Password
            });

            ack.IsSuccess = true;
            ack.Data = new LoginResultModel()
            {
                Token = token,
                DisplayName = newUser.DisplayName,
                UserId = newUser.Id,
                AvatarFileId = newUser.AvatarFileId
            };
            return ack;
        }
        public async Task<Acknowledgement<List<UserViewModel>>> GetOnlineUserList( Guid currentUserId)
        {
            var ack = new Acknowledgement<List<UserViewModel>>();
            var context = DbContext;
            var onlines = ChatHub.GetAllUserOnlines().Where(i=>i != currentUserId).ToList();

            var predicate = PredicateBuilder.New<User>(false);
            predicate = predicate.And(i => onlines.Contains(i.Id));
         
            var users = await context.Users.Where(predicate).ToListAsync();
            var result = users.Select(i =>
            {
                var u = new UserViewModel();
                u.MapDTOUser(i);
                u.IsOnline = true;
                return u;
            }).Take(50).ToList();

            ack.IsSuccess = true;
            ack.Data = result;
            return ack;
        }
    }
}
