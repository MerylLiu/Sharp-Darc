namespace Darc.Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using CommonServiceLocator.WindsorAdapter;
    using Core;
    using Dapper;
    using Microsoft.Practices.ServiceLocation;

    public class ServiceLocatorInitializer
    {
        public static void Init()
        {
            IWindsorContainer container = new WindsorContainer();
            //AddCustomRepositoriesTo(container);
            AddGenericRepositoriesTo(container);
            AddQueryObjectsTo(container);
            AddTasksTo(container);

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

        private static void AddTasksTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromAssemblyNamed("Darc.Commands")
                    .BasedOn(typeof (CommandBase))
                    .WithService.FirstInterface());
        }
    }
}