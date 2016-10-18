namespace Darc.Tests
{
    using Castle.DynamicProxy;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using CommonServiceLocator.WindsorAdapter;
    using Core;
    using Core.Contracts;
    using Core.Interceptor;
    using Dapper;
    using Dapper.Common;
    using Microsoft.Practices.ServiceLocation;

    public class ServiceLocatorInitializer
    {
        public static void Init()
        {
            IWindsorContainer container = new WindsorContainer();

            AddCommentsTo(container);
            AddGenericRepositoriesTo(container);
            AddQueryObjectsTo(container);
            AddTasksTo(container);

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
        }

        private static void AddCommentsTo(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof (IDataContext))
                    .ImplementedBy(typeof (DataContext))
                    .Named("DataSession"));

            container.Register(
                Component.For<IInterceptor>()
                    .ImplementedBy<TransactionInterceptor>()
                    .Named("Transaction"));
        }

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
                AllTypes.FromAssemblyNamed("Darc.Queries")
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