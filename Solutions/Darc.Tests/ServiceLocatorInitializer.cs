using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;

namespace Darc.Tests
{
    using Darc.Dapper;

    public class ServiceLocatorInitializer
    {
        public static void Init()
        {
            IWindsorContainer container = new WindsorContainer();
            //AddCustomRepositoriesTo(container);
            AddQueryObjectsTo(container);


            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
        }

        //private static void AddCustomRepositoriesTo(IWindsorContainer container)
        //{
        //    container.Register(
        //        AllTypes.FromAssemblyNamed("FS.Repository")
        //                .BasedOn<Repository.Repository>()
        //                .WithService.DefaultInterfaces());
        //}

        private static void AddQueryObjectsTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromAssemblyNamed("Narc.Web.Common")
                        .BasedOn(typeof(DapperQuery))
                        .WithService.DefaultInterfaces());
        }
    }
}