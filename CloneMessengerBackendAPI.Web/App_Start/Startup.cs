using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Interfaces;
using CloneMessengerBackendAPI.Service.Serviecs;
using CloneMessengerBackendAPI.Web.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(CloneMessengerBackendAPI.Web.App_Start.Startup))]

namespace CloneMessengerBackendAPI.Web.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            ConfigAutofac(app);
            ConfigureOAuthTokenConsumption(app);
           
            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/api/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                hubConfiguration.EnableDetailedErrors = true;
                map.RunSignalR(hubConfiguration);
            });
        }
        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"])));

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateAudience = true,
                ValidAudience = ConfigurationManager.AppSettings["config:JwtAudience"],
                ValidateIssuer = true,
                ValidIssuer = ConfigurationManager.AppSettings["config:JwtIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
            };
            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = validationParameters
            });
        }
        private void ConfigAutofac(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()); //Register WebApi Controllers

            builder.RegisterType<CloneMessengerDbContext>().AsSelf().InstancePerRequest();

            builder.RegisterType<ChatHub>().As<IChatHubService>();
            // Services
            builder.RegisterAssemblyTypes(typeof(UserServices).Assembly)
             .Where(t => t.Name.EndsWith("UserServices"))
             .AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterAssemblyTypes(typeof(MessageServices).Assembly)
               .Where(t => t.Name.EndsWith("MessageServices"))
               .AsImplementedInterfaces().InstancePerRequest();
           

            Autofac.IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver((IContainer)container); //Set the WebApi DependencyResolver

        }

    }
}
