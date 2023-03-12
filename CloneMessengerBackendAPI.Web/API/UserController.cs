using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using EHealth.Web.Helper;
using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace CloneMessengerBackendAPI.Web.API
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class UserController : BaseAPIController
    {
        public UserController(IUserService userService, IMessageService messageService) : base(userService, messageService)
        {
        }

        [AllowAnonymous]
        // GET: User
        public async Task<IHttpActionResult> Login(LoginModel model)
        {
            var result = await UserServices.Login(model);
            if (result.IsSuccess && result.Data != null)
            {
                result.Data.AvatarSrc = APIHelper.GetFileUrl(result.Data.AvatarFileId);
            }
            return MapToIHttpActionResult(result);
        }  
        [AllowAnonymous]
        [HttpPost]
        // GET: User
        public async Task<IHttpActionResult> Register(RegisterModel post)
        {
            var result = await UserServices.Register(post);
            if (result.IsSuccess && result.Data != null)
            {
                result.Data.AvatarSrc = APIHelper.GetFileUrl(result.Data.AvatarFileId);
            }
            return MapToIHttpActionResult(result);
        }
        


        [HttpGet]
        public async Task<IHttpActionResult> GetOnlineUserList()
        {
            var result = await UserServices.GetOnlineUserList(GetCurrentUserModel().Id);
            if (result.IsSuccess && result.Data != null)
            {
                result.Data.ForEach(i =>
                {
                    i.AvatarFileSrc = APIHelper.GetFileUrl(i.AvatarFileId);
                });
            }
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
        #region FACEBOOK LOGIN
        private string RediredtUrl
        {
            get
            {
                string url = Url.Link("Default", new { controller = "user", action = "FacebookCallback" });
                return url;

                //var uriBuilder = new UriBuilder(RediredtUrl.G);

                //uriBuilder.Query = null;

                //uriBuilder.Fragment = null;

                //uriBuilder.Path = Url.Action("FacebookCallback");

                //return uriBuilder.Uri;
            }


        }

        [AllowAnonymous]
        [HttpGet]
        // GET: User
        public IHttpActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                clien_id = "766632724798618",
                client_secret = "91f518537d8dadaa20808d4093e1f382",
                redirect_uri = RediredtUrl,
                response_type = "code",
                scope = "email"

            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> FacebookCallback(string code)

        {

            var fb = new FacebookClient();

            dynamic result = fb.Post("oauth/access_token", new

            {

                clien_id = "766632724798618",
                client_secret = "91f518537d8dadaa20808d4093e1f382",
                redirect_uri = RediredtUrl,
                code = code
            });

            var accessToken = result.access_token;


            fb.AccessToken = accessToken;

            dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");

            string email = me.email;


            return Ok();

        }
        #endregion
    }
}
