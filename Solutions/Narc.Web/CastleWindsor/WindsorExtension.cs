namespace Narc.Web.CastleWindsor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using Castle.Core;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    public static class WindsorExtension
    {
        public static BasedOnDescriptor FirstNonGenericCoreInterface(this ServiceDescriptor descriptor,
                                                                     string interfaceNamespace)
        {
            return descriptor.Select(
                delegate(Type type, Type[] baseType)
                    {
                        IEnumerable<Type> interfaces =
                            type.GetInterfaces().Where(
                                t => t.IsGenericType == false && t.Namespace.StartsWith(interfaceNamespace));

                        if (interfaces.Count() > 0)
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
            foreach (Type type in controllerTypes)
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
            foreach (Assembly assembly in assemblies)
            {
                container.RegisterControllers(assembly.GetExportedTypes());
            }
            return container;
        }
    }
}