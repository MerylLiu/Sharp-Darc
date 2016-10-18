namespace Darc.Core.Castle
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using global::Castle.Windsor;

    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer _container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container is null");
            }

            _container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404,
                    string.Format(
                        "The controller for path '{0}' could not be found or it does not implement IController.",
                        requestContext.HttpContext.Request.Path));
            }

            return (IController) _container.Resolve(controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;
            disposable?.Dispose();

            _container.Release(controller);
        }
    }
}