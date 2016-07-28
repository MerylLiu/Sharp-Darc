namespace Narc.Web.CastleWindsor
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Castle.Windsor;

    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IWindsorContainer container;

        public WindsorControllerFactory(IWindsorContainer _container)
        {
            if (_container == null)
            {
                throw new ArgumentNullException("container");
            }

            container = _container;
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

            return (IController) container.Resolve(controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            var disposable = controller as IDisposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }

            container.Release(controller);
        }
    }
}