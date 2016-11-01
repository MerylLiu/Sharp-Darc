using Darc.Web;
using Microsoft.Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace Darc.Web
{
    using System.Web.Mvc;
    using Areas.Admin.Controllers;
    using Castle.MicroKernel.Registration;
    using Controllers;
    using Core;
    using Core.Castle;
    using Dapper;
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            RegisterComponents();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        private void RegisterComponents()
        {
            var defaultControllers = new[]
            {
                typeof (HomeController).Assembly,
                typeof (MainController).Assembly
            };

            CastleRegistrer.Initialize(defaultControllers).AddComponentsTo(p =>
            {
                p.Register(AllTypes.FromAssemblyNamed("Darc.Commands")
                    .BasedOn(typeof (CommandBase))
                    .WithService.FirstInterface());

                p.Register(AllTypes.FromAssemblyNamed("Darc.Queries")
                    .BasedOn(typeof (DapperQuery))
                    .WithService.DefaultInterfaces());
            });
        }
    }
}