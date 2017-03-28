using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Outcomes.v1;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace LtiLibrary.AspNet.Outcomes.v1
{
    public class ImsxXmlMediaTypeFormatter : IInputFormatter, IOutputFormatter
    {
        // The XSD code generator only creates one imsx_POXEnvelopeType which has the 
        // imsx_POXEnvelopeRequest root element. The IMS spec says the root element
        // should be imsx_POXEnvelopeResponse in the response.

        // Create two serializers: one for requests and one for responses.
        private static readonly XmlSerializer ImsxRequestSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType),
            null, null, new XmlRootAttribute("imsx_POXEnvelopeRequest"),
                    "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");
        private static readonly XmlSerializer ImsxResponseSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType),
            null, null, new XmlRootAttribute("imsx_POXEnvelopeResponse"),
                    "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.ContentType.ToString() == "application/xml";
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var response = context.HttpContext.Response;
            response.ContentType = "application/xml";

            using (var writer = context.WriterFactory(response.Body, Encoding.UTF8))
            {
                ImsxResponseSerializer.Serialize(writer, context.Object);
                await writer.FlushAsync();
            }
        }

        public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var request = context.HttpContext.Request;
            if (request.ContentLength == 0)
            {
                if (context.ModelType.GetTypeInfo().IsValueType)
                {
                    return InputFormatterResult.SuccessAsync((Activator.CreateInstance(context.ModelType)));
                }
                else
                {
                    return InputFormatterResult.SuccessAsync(null);
                }
            }

            var encoding = Encoding.UTF8;

            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                var model = ImsxRequestSerializer.Deserialize(reader);
                return InputFormatterResult.SuccessAsync(model);
            }
        }

        public bool CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            return contentType == null || contentType == "application/xml";
        }
    }
}
