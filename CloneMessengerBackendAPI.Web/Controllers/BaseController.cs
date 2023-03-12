using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using CloneMessengerBackendAPI.Web.Hubs;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace CloneMessengerBackendAPI.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly IMessageService messageService;
        private readonly IUserService userService;
        public BaseController(IUserService userService, IMessageService messageService)
        {
            this.messageService = messageService;
            this.userService = userService;
        }
       
        public IMessageService MessageServices => messageService;
        public IUserService UserServices => userService;
    }
}
