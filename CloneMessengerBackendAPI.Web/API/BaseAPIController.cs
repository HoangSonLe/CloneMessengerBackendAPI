using CloneMessengerBackendAPI.Service.Serviecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CloneMessengerBackendAPI.Web.API
{
    public class BaseAPIController : ApiController
    {
        public IMessageServices MessageServices;
        public BaseAPIController(IMessageServices messageServices)
        {
            MessageServices = messageServices;
        }
    }
}