using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace CloneMessengerBackendAPI.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // setup camel-case for property names
            var settings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}
