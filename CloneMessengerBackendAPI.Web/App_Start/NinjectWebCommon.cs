[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CloneMessengerBackendAPI.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CloneMessengerBackendAPI.Web.App_Start.NinjectWebCommon), "Stop")]

namespace CloneMessengerBackendAPI.Web.App_Start
{
    using System;
    using System.Web;
    using CloneMessengerBackendAPI.Model.Model;
    using CloneMessengerBackendAPI.Service.Interfaces;
    using CloneMessengerBackendAPI.Service.Serviecs;
    using CloneMessengerBackendAPI.Web.Hubs;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                System.Web.Http.GlobalConfiguration.Configuration.Dependency‌​Resolver = new Ninject.Web.WebApi.NinjectDependencyResolver(kernel);
                System.Web.Mvc.DependencyResolver.SetResolver(new Ninject.Web.Mvc.NinjectDependencyResolver(kernel));
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IChatHubService>().To<ChatHub>();
            kernel.Bind<IUserService>().To<UserServices>();
            kernel.Bind<IMessageService>().To<MessageServices>();
        }
    }
}