using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CloneMessengerBackendAPI.Web.API
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : BaseAPIController
    {
        public UserController(IUserService userService, IMessageService messageService) : base(userService, messageService)
        {
        }

        [AllowAnonymous]
        // GET: User
        public async Task<IHttpActionResult> Login(LoginModel model)
        {
            var result =await UserServices.Login(model);
            return MapToIHttpActionResult(result);
        }
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetJwt()
        {
            var current = HttpContext.Current.User.Identity;
            var identity = (ClaimsIdentity)current;
            var token = Authentication.GetJwt(identity);
            var result = new Acknowledgement<string>()
            {
                Data = token,
                IsSuccess = true
            };
            return MapToIHttpActionResult(result);
        }
       
    }
}