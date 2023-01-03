using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Models.BaseModels;
using CloneMessengerBackendAPI.Service.Serviecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CloneMessengerBackendAPI.Web.API
{
    public class BaseAPIController : ApiController
    {
        private readonly IServiceLocator serviceLocator;
        public BaseAPIController(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
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
        public IMessageService MessageServices => serviceLocator.MessageService;
        public IUserService UserServices => serviceLocator.UserService;
    }
}