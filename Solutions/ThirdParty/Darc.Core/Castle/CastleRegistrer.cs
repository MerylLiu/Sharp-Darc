namespace Darc.Core.Castle
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using CommonServiceLocator.WindsorAdapter;
    using Contracts;
    using global::Castle.DynamicProxy;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using Interceptor;
    using Microsoft.Practices.ServiceLocation;

    public class CastleRegistrer
    {
        private static IWindsorContainer _container;

        public CastleRegistrer(IWindsorContainer container)
        {
            _container = container;
        }

        public static CastleRegistrer Initialize(params Assembly[] controllerAssemblies)
        {
            IWindsorContainer container = new WindsorContainer();
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(controllerAssemblies);

            if (controllerAssemblies != null && controllerAssemblies.Any())
            {
                AddDefaultComponentsTo(container);
            }

            return new CastleRegistrer(container);
        }

        private static void AddDefaultComponentsTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes.FromAssemblyNamed("Darc.Dapper")
                    .BasedOn(typeof (IDataContext))
                    .WithService.FirstInterface());

            container.Register(
                Component.For<IInterceptor>()
                    .ImplementedBy<TransactionInterceptor>()
                    .Named("Transaction"));

            container.Register(
                Component.For<IInterceptor>()
                    .ImplementedBy<TasksInterceptor>()
                    .Named("Tasks"));

            container.Register(
                Component.For(typeof (ICommandProcessor))
                    .ImplementedBy(typeof (CommandProcessor))
                    .Named("commandProcessor"));

            SetServiceLocator(container);
        }

        public virtual void AddComponentsTo(Action<IWindsorContainer> action)
        {
            action(_container);
            SetServiceLocator(_container);
        }

        private static void SetServiceLocator(IWindsorContainer container)
        {
            var windsorServiceLocator = new WindsorServiceLocator(_container);
            ServiceLocator.SetLocatorProvider(() => windsorServiceLocator);
        }
    }
}