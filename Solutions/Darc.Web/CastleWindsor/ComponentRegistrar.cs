namespace Darc.Web.CastleWindsor
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Dapper;
    using Domain;

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
                AllTypes.FromAssemblyNamed("Darc.Tasks")
                        .Pick()
                        .If(t => t.Name.EndsWith("Tasks"))
                        .WithService.FirstNonGenericCoreInterface("Darc.Domain"));
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
                AllTypes.FromAssemblyNamed("Darc.Web.Common")
                        .BasedOn(typeof(DapperQuery))
                        .WithService.DefaultInterfaces());
        }

        private static void AddHandlersTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromAssemblyNamed("Darc.Tasks")
                        .BasedOn(typeof (ICommandHandler<>))
                        .WithService.FirstInterface());
        }
    }
}