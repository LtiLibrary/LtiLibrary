using LtiLibrary.NetCore.Outcomes.v1;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// Use this ModelBinder to deserialize imsx_POXEnvelopeRequest XML. For example,
    /// <code>public ImsxXmlMediaTypeResult Post([ModelBinder(BinderType = typeof(ImsxXmlMediaTypeModelBinder))] imsx_POXEnvelopeType request)</code>
    /// </summary>
    public class ImsxXmlMediaTypeModelBinder : IModelBinder
    {
        private static readonly XmlSerializer ImsxRequestSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType),
            null, null, new XmlRootAttribute("imsx_POXEnvelopeRequest"),
            "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            if (bindingContext.HttpContext?.Request == null || !bindingContext.HttpContext.Request.ContentType.Equals("application/xml"))
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

            await Task.Yield();
        }
    }
}
