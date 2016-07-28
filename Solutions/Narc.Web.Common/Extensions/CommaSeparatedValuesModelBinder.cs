namespace Narc.Web.Common.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    public class CommaSeparatedValuesModelBinder : IModelBinder
    {
        private static readonly MethodInfo ToArrayMethod = typeof (Enumerable).GetMethod("ToArray");

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null || string.IsNullOrEmpty(value.AttemptedValue))
            {
                return null;
            }

            Type valueType = bindingContext.ModelType.GetElementType() ??
                             bindingContext.ModelType.GetGenericArguments().FirstOrDefault();

            if (valueType != null && valueType.GetInterface(typeof (IConvertible).Name) != null)
            {
                var list = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(valueType));

                foreach (string splitValue in value.AttemptedValue.Split(new[] {','}))
                {
                    if (!String.IsNullOrWhiteSpace(splitValue))
                        list.Add(Convert.ChangeType(splitValue, valueType));
                }

                if (bindingContext.ModelType.IsArray)
                    return ToArrayMethod.MakeGenericMethod(valueType).Invoke(this, new[] {list});
                else
                    return list;
            }

            return null;
        }
    }
}