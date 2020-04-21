using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// AspNetCore.Mvc output formatter returns an imsx_POXEnvelopeType with an imsx_POXEnvelopeResponse.
    /// </summary>
    internal class ImsxXmlMediaTypeOutputFormatter : IOutputFormatter
    {
        // The XSD code generator only creates one imsx_POXEnvelopeType which has the 
        // imsx_POXEnvelopeRequest root element. The IMS spec says the root element
        // should be imsx_POXEnvelopeResponse in the response.

        // Create a serializer that will replace the root node with imsx_POXEnvelopeResponse.

        private static readonly XmlSerializer ImsxResponseSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType),
            null, null, new XmlRootAttribute("imsx_POXEnvelopeResponse"),
                    "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.ObjectType == typeof(imsx_POXEnvelopeType);
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var response = context.HttpContext.Response;
            response.ContentType = context.ContentType.Value ?? "application/xml";


            using (var ms = new MemoryStream())
            {
                using (var writer = context.WriterFactory(ms, Encoding.UTF8))
                {
                    ImsxResponseSerializer.Serialize(writer, context.Object);
                    await writer.FlushAsync().ConfigureAwait(false);
                }

                await response.Body.WriteAsync(ms.ToArray(), 0, (int)ms.Length);
            }
        }
    }
}
