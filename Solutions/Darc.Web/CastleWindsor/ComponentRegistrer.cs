namespace Darc.Web.CastleWindsor
{
    using Castle.DynamicProxy;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Core;
    using Core.Contracts;
    using Core.Interceptor;
    using Dapper;
    using Dapper.Common;

    public class ComponentRegistrer
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddCommentsTo(container);
            AddRepositoriesTo(container);
            AddQueryObjectsTo(container);
            AddTasksTo(container);
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

        private static void AddTasksTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromAssemblyNamed("Darc.Commands")
                    .BasedOn(typeof (CommandBase))
                    .WithService.FirstInterface());
        }

        private static void AddRepositoriesTo(IWindsorContainer container)
        {
            //container.Register(
            //    AllTypes.FromAssemblyNamed("Darc.Infrastructure")
            //            .BasedOn(typeof (IRepositoryWithTypedId<,>))
            //            .WithService.FirstNonGenericCoreInterface("Darc.Domain"));

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
    }
}