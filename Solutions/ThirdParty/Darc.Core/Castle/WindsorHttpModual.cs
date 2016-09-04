namespace FS.Framework.Webform
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.UI;
    using Microsoft.Practices.ServiceLocation;

    public class WindsorHttpModual : IHttpModule
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> InjectedProperties =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        private HttpApplication _context;

        #region Implementation of IHttpModule

        /// <summary>
        ///     Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        ///     An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties,
        ///     and events common to all application objects within an ASP.NET application
        /// </param>
        public void Init(HttpApplication context)
        {
            _context = context;
            _context.PreRequestHandlerExecute += InjectProperties;
            _context.EndRequest += ReleaseComponents;
        }

        /// <summary>
        ///     Disposes of the resources (other than memory) used by the module that implements
        ///     <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        private List<object> ResolvedComponents
        {
            get { return (List<object>) HttpContext.Current.Items["ResolvedComponents"]; }
            set { HttpContext.Current.Items["ResolvedComponents"] = value; }
        }

        private void InjectProperties(object sender, EventArgs e)
        {
            var currentPage = _context.Context.CurrentHandler as Page;
            if (currentPage != null)
            {
                InjectProperties(currentPage);
                currentPage.InitComplete += delegate { InjectUserControls(currentPage); };
            }
        }

        private void InjectUserControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is UserControl)
                {
                    InjectProperties(control);
                }
                InjectUserControls(control);
            }
        }

        private void InjectProperties(Control control)
        {
            ResolvedComponents = new List<object>();
            Type pageType = control.GetType();

            PropertyInfo[] properties;
            if (!InjectedProperties.TryGetValue(pageType, out properties))
            {
                properties = control.GetType().GetProperties()
                                    .ToArray();
                InjectedProperties.TryAdd(pageType, properties);
            }

            foreach (PropertyInfo property in properties)
            {
                try
                {
                    object component = GetInstance(property.PropertyType);
                    property.SetValue(control, component, null);
                    ResolvedComponents.Add(component);
                }
                catch (Exception)
                {
                }
            }
        }

        private void ReleaseComponents(object sender, EventArgs e)
        {
            List<object> resolvedComponents = ResolvedComponents;
            if (resolvedComponents != null)
            {
                foreach (object component in ResolvedComponents)
                {
                    Release(component);
                }
            }
        }

        private object GetInstance(Type type)
        {
            try
            {
                return ServiceLocator.Current.GetInstance(type);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void Release(object comment)
        {
        }
    }
}