using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CloneMessengerBackendAPI.Service.Models.ViewModels
{
    public class UserViewModel : UserModel
    {
        public UserViewModel() { }
        public UserViewModel(UserModel u)
        {
            Id = u.Id;
            AvatarFileId = u.AvatarFileId;
            DisplayName = u.DisplayName;
        }
        public bool IsOnline { get; set; }
      

    }
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public Guid? AvatarFileId { get; set; }
        public string AvatarFileSrc { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserModel ParseClaim(ClaimsPrincipal claimsPrincipal)
        {
            var result = new UserModel()
            {
                Id = Guid.Parse(claimsPrincipal.Claims.Where(i => i.Type == ClaimTypes.NameIdentifier).First().Value),
                Username = claimsPrincipal.Claims.Where(i => i.Type == ClaimTypes.Name).First().Value,
                DisplayName = claimsPrincipal.Claims.Where(i => i.Type == ClaimTypes.GivenName).First().Value,
                Password = claimsPrincipal.Claims.Where(i => i.Type == ClaimTypes.UserData).First().Value,
            };
            return result;
        }
    }
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }    
    }
    public class RegisterModel : LoginModel
    {
        public string DisplayName { get; set;}
        public Guid? AvatarFileId { get; set; }

    }
    public class LoginResultModel
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public Guid? AvatarFileId { get; set; }
        public string AvatarSrc { get; set; }
    }
}
