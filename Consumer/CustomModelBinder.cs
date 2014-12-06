using Consumer.Extensions;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace Consumer
{
    /// <summary>
    /// This custom ModelBinder will use the System.Runtime.Serialization.DataMemberAttribute to
    /// override the default name mapping.
    /// </summary>
    public class CustomModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var performValidation = controllerContext.Controller.ValidateRequest &&
                                    bindingContext.ModelMetadata.RequestValidationEnabled;

            var propertyBinderAttribute = propertyDescriptor
                .Attributes
                .OfType<DataMemberAttribute>()
                .FirstOrDefault();

            var modelName = propertyBinderAttribute == null ? bindingContext.ModelName : propertyBinderAttribute.Name;

            var result = performValidation 
                ? bindingContext.ValueProvider.GetValue(modelName) 
                : bindingContext.GetUnvalidatedValue(modelName);

            if (result == null)
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
            else
            {
                propertyDescriptor.SetValue(bindingContext.Model, result.AttemptedValue);
            }
        }
    }
}