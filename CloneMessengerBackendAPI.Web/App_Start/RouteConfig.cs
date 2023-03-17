using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CloneMessengerBackendAPI.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
              name: "Home",
              url: "Home/{action}/{id}",
              defaults: new { controller = "File", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "Values",
               url: "Values/{action}/{id}",
               defaults: new { controller = "File", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "User",
               url: "User/{action}/{id}",
               defaults: new { controller = "File", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Chat",
               url: "Chat/{action}/{id}",
               defaults: new { controller = "File", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "File",
               url: "File/{action}/{id}",
               defaults: new { controller = "File", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
