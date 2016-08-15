namespace Darc.Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using CommonServiceLocator.WindsorAdapter;
    using Dapper;
    using Domain;
    using Infrastructure.Castle;
    using Microsoft.Practices.ServiceLocation;

    public class ServiceLocatorInitializer
    {
        public static void Init()
        {
            IWindsorContainer container = new WindsorContainer();
            //AddCustomRepositoriesTo(container);
            AddGenericRepositoriesTo(container);
            AddQueryObjectsTo(container);
            AddHandlersTo(container);


            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
        }

        //private static void AddCustomRepositoriesTo(IWindsorContainer container)
        //{
        //    container.Register(
        //        AllTypes.FromAssemblyNamed("FS.Repository")
        //                .BasedOn<Repository.Repository>()
        //                .WithService.DefaultInterfaces());
        //}

        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof (ICommandProcessor))
                    .ImplementedBy(typeof (CommandProcessor))
                    .Named("commandProcessor"));
        }

        private static void AddQueryObjectsTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromAssemblyNamed("Darc.Web.Common")
                    .BasedOn(typeof (DapperQuery))
                    .WithService.DefaultInterfaces());
        }

        private static void AddHandlersTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromAssemblyNamed("Darc.Tasks")
                    .BasedOn(typeof (CommandBase))
                    .WithService.FirstInterface());

            /*container.Register(
                AllTypes
                    .FromAssemblyNamed("Darc.Tasks")
                    .Pick().If(t => t.Name.EndsWith("Command"))
                    .WithService.FirstNonGenericCoreInterface("Darc.Domain"));*/
        }
    }
}