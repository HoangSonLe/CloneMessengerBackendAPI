using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using CloneMessengerBackendAPI.Model.Model;
using CloneMessengerBackendAPI.Service.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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


            // Services
            builder.RegisterAssemblyTypes(typeof(IoCServiceLocator).Assembly)
           .Where(t => t.Name.EndsWith("ServiceLocator"))
           .AsImplementedInterfaces().InstancePerRequest();
            //builder.RegisterAssemblyTypes(typeof(MessageServices).Assembly)
            //   .Where(t => t.Name.EndsWith("MessageServices"))
            //   .AsImplementedInterfaces().InstancePerRequest();
            //builder.RegisterAssemblyTypes(typeof(UserServices).Assembly)
            //  .Where(t => t.Name.EndsWith("UserServices"))
            //  .AsImplementedInterfaces().InstancePerRequest();

            Autofac.IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver((IContainer)container); //Set the WebApi DependencyResolver

        }

    }
}
