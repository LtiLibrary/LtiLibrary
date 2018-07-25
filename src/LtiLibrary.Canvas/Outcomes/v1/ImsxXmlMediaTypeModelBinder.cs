using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LtiLibrary.Canvas.Lti.v1;

namespace LtiLibrary.Canvas.Outcomes.v1
{
    /// <summary>
    /// Use this ModelBinder to deserialize imsx_POXEnvelopeRequest XML. For example,
    /// <code>public ImsxXmlMediaTypeResult Post([ModelBinder(BinderType = typeof(ImsxXmlMediaTypeModelBinder))] imsx_POXEnvelopeType request)</code>
    /// </summary>
    internal class ImsxXmlMediaTypeModelBinder : IModelBinder
    {
        private static readonly XmlSerializer ImsxRequestSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType),
            null, null, new XmlRootAttribute("imsx_POXEnvelopeRequest"),
            "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            if (bindingContext.HttpContext?.Request == null || !bindingContext.HttpContext.Request.ContentType.Contains("application/xml"))
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            else
            {
                try
                {
                    using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
                    {
                        var model = ImsxRequestSerializer.Deserialize(reader);
                        bindingContext.Result = ModelBindingResult.Success(model);
                    }
                }
                catch
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }

            // To avoid warning that there are no await call in this async method
            await Task.Yield();
        }
    }
}
