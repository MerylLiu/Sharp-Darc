namespace Darc.Web.CastleWindsor
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Core;
    using Core.Filters;
    using Dapper;

    public class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddCustomRepositoriesTo(container);
            AddHandlersTo(container);
            AddGenericRepositoriesTo(container);
            AddQueryObjectsTo(container);
            AddTaskTo(container);
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

        private static void AddHandlersTo(IWindsorContainer container)
        {
                container.Register(
                    AllTypes.FromAssemblyNamed("Darc.Core")
                            .BasedOn(typeof (IFilterAttribute))
                            .WithService.FirstInterface());
        }
    }
}