﻿using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Models.ViewModels;
using CloneMessengerBackendAPI.Service.Serviecs;
using CloneMessengerBackendAPI.Web.Hubs;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CloneMessengerBackendAPI.Web.API
{
    public class BaseAPIController : ApiController
    {
        private readonly IMessageService messageService;
        private readonly IUserService userService;
        public BaseAPIController(IUserService userService, IMessageService messageService)
        {
            this.messageService = messageService;
            this.userService = userService;
        }
        public IHttpActionResult MapToIHttpActionResult(Acknowledgement ack)
        {
            if (ack == null)
            {
                return Content(HttpStatusCode.NotFound, "ReturnData is null");
            }
            if (ack.IsSuccess == false)
            {
                return Content(HttpStatusCode.NotFound, ack.ErrorMessage);
            }
            return Ok(ack);
        }
        public IHttpActionResult MapToIHttpActionResult<T>(Acknowledgement<T> ack)
        {
            if(ack == null)
            {
                return Content(HttpStatusCode.NotFound,"ReturnData is null");
            }
            if(ack.IsSuccess == false)
            {
                return Content(HttpStatusCode.NotFound, ack.ErrorMessage);
            }
            return Ok(ack);
        }
        protected IHubConnectionContext<IChatHub> ChatHubContext
        {
            get
            {
                var x = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub, IChatHub>();
                return x.Clients;
            }
        }
        protected UserModel GetCurrentUserModel()
        {
            var current = HttpContext.Current.User;
            var identity = (ClaimsPrincipal)current;
            var result = (new UserModel()).ParseClaim(identity);
            return result;
        }
        public IMessageService MessageServices => messageService;
        public IUserService UserServices => userService;
    }
}