namespace Darc.Core.Filters
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class FilterAttribute : Attribute, IFilterAttribute
    {
        public virtual void OnExecuting()
        {
        }

        public virtual void OnExecuted()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FilterableAttribute : FilterContext, IFilterAttribute
    {
        public FilterableAttribute() : base(string.Empty)
        {
        }

        protected FilterableAttribute(string name)
            : base(name)
        {
        }

        public virtual void OnExecuting()
        {
        }

        public virtual void OnExecuted()
        {
        }
    }
}