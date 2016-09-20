namespace Darc.Web.CastleWindsor
{
    using Castle.DynamicProxy;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Core;
    using Dapper;
    using Infrastructure.Extensions;

    public class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddCommentsTo(container);
            AddCustomRepositoriesTo(container);
            AddGenericRepositoriesTo(container);
            AddQueryObjectsTo(container);
            AddTaskTo(container);
        }

        private static void AddCommentsTo(IWindsorContainer container)
        {
            container.Register(
                Component.For<IInterceptor>()
                    .ImplementedBy<Logger>()
                    .Named("Logger"));
        }

        private static void AddTaskTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromAssemblyNamed("Darc.Commands")
                    .BasedOn(typeof (CommandBase))
                    .WithService.FirstInterface());
        }

        private static void AddCustomRepositoriesTo(IWindsorContainer container)
        {
            //container.Register(
            //    AllTypes.FromAssemblyNamed("Darc.Infrastructure")
            //            .BasedOn(typeof (IRepositoryWithTypedId<,>))
            //            .WithService.FirstNonGenericCoreInterface("Darc.Domain"));
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
    }
}