using System.Web.Mvc;

namespace Consumer.Extensions
{
    public static class ModelBindingContextExtensions
    {
        public static ValueProviderResult GetUnvalidatedValue(this ModelBindingContext bindingContext, string key)
        {
            var unvalidatedValueProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;
            return unvalidatedValueProvider == null 
                ? bindingContext.ValueProvider.GetValue(key) 
                : unvalidatedValueProvider.GetValue(key, true);
        }
    }
}