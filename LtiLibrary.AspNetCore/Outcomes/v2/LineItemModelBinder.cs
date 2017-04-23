using System;
using System.IO;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Outcomes.v2;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    internal class LineItemModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            if (bindingContext.HttpContext?.Request == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            else
            {
                try
                {
                    using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        var model = JsonConvert.DeserializeObject<LineItem>(body);
                        bindingContext.Result = ModelBindingResult.Success(model);
                    }
                }
                catch
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
        }
    }
}
