using System;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LtiLibrary.Core.Outcomes.v1;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IO;

namespace LtiLibrary.AspNet.Outcomes.v1
{
    public class ImsxXmlMediaTypeModelBinder : IModelBinder
    {
        private static readonly XmlSerializer ImsxRequestSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType),
            null, null, new XmlRootAttribute("imsx_POXEnvelopeRequest"),
            "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                var model = ImsxRequestSerializer.Deserialize(reader);
                bindingContext.Result = ModelBindingResult.Success(model);
            }

            return null;
        }
    }
}
