using Darc.Web;
using Microsoft.Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace Darc.Web
{
    using System.Web.Mvc;
    using Areas.Admin.Controllers;
    using Castle.Windsor;
    using CastleWindsor;
    using CommonServiceLocator.WindsorAdapter;
    using Controllers;
    using Microsoft.Practices.ServiceLocation;
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            InitializeServiceLocator();

            ConfigureAuth(app);
        }

        protected void InitializeServiceLocator()
        {
            IWindsorContainer container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof (HomeController).Assembly);
            container.RegisterControllers(typeof (MainController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            var windsorServiceLocator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => windsorServiceLocator);
        }
    }
}