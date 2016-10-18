namespace Darc.Core.Castle
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using global::Castle.Core;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;

    public static class WindsorExtension
    {
        public static BasedOnDescriptor FirstNonGenericCoreInterface(this ServiceDescriptor descriptor,
            string interfaceNamespace)
        {
            return descriptor.Select(
                delegate(Type type, Type[] baseType)
                {
                    var interfaces = type.GetInterfaces().Where(
                        t => t.IsGenericType == false
                             && t.Namespace.StartsWith(interfaceNamespace)).ToList();

                    if (interfaces.Any())
                    {
                        return new[] {interfaces.ElementAt(0)};
                    }

                    return null;
                });
        }

        public static IWindsorContainer RegisterController<T>(this IWindsorContainer container) where T : IController
        {
            container.RegisterControllers(typeof (T));
            return container;
        }

        public static IWindsorContainer RegisterControllers(this IWindsorContainer container,
            params Type[] controllerTypes)
        {
            foreach (var type in controllerTypes)
            {
                if (type != null
                    && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                    && !type.IsAbstract
                    && typeof (IController).IsAssignableFrom(type))
                {
                    container.Register(
                        Component.For(type).Named(type.FullName.ToLower()).LifeStyle.Is(LifestyleType.Transient));
                }
            }

            return container;
        }

        public static IWindsorContainer RegisterControllers(this IWindsorContainer container,
            params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                container.RegisterControllers(assembly.GetExportedTypes());
            }
            return container;
        }
    }
}