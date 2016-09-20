namespace Darc.Core.Castle
{
    using System;
    using System.Linq;
    using System.Reflection;
    using global::Castle.Core;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;

    public static class WindsorExtensions
    {
        public static BasedOnDescriptor FirstNonGenericCoreInterface(
            this ServiceDescriptor descriptor, string interfaceNamespace)
        {
            return descriptor.Select(
                delegate(Type type, Type[] baseType)
                {
                    var interfaces =
                        type.GetInterfaces()
                            .Where(t => t.IsGenericType == false
                                        && t.Namespace.StartsWith(interfaceNamespace));

                    if (interfaces.Any())
                    {
                        return new[] {interfaces.ElementAt(0)};
                    }

                    return null;
                });
        }

        public static IWindsorContainer RegisterPage<T>(this IWindsorContainer container)
        {
            container.RegisterPages(typeof (T));

            return container;
        }

        public static IWindsorContainer RegisterPages(this IWindsorContainer container, params Type[] pageTypes)
        {
            foreach (var type in pageTypes)
            {
                container.Register(
                    Component.For(type).Named(type.FullName.ToLower()).LifeStyle.Is(LifestyleType.Transient));
            }

            return container;
        }

        public static IWindsorContainer RegisterPages(this IWindsorContainer container, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                container.RegisterPages(assembly.GetExportedTypes());
            }
            return container;
        }
    }
}