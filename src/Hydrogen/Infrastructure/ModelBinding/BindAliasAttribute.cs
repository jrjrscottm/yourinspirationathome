using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Hydrogen.Infrastructure.ModelBinding
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class BindAliasAttribute : Attribute
    {
        public BindAliasAttribute(string alias)
        {
            Alias = alias;
        }
        public string Alias { get; private set; }
    }

    public class FromAliasModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            
            var propName = bindingContext.ModelMetadata.PropertyName;

            if (bindingContext.Model == null || propName == null)
                return Task.FromResult(ModelBindingResult.Failed());

            var propInfo = bindingContext.Model.GetType().GetProperty(propName);
            
            var attributes = propInfo.GetCustomAttributes(typeof(BindAliasAttribute), true).Cast<BindAliasAttribute>();

            foreach (var attribute in attributes)
            {
                StringValues value;

                if (bindingContext.HttpContext.Request.Form.TryGetValue(attribute.Alias, out value))
                {
                    //TODO: Support types and typed collections.
                    propInfo.SetValue(bindingContext.Model, value.FirstOrDefault());
                    return Task.FromResult(ModelBindingResult.Success(bindingContext.Model));
                }
            }

            return Task.FromResult(ModelBindingResult.Failed());
        }
    }

    public class HydrogenModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return new FromAliasModelBinder();
        }
    }
}
