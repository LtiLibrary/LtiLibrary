using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LtiLibrary.Models;

namespace LtiLibrary.Outcomes
{
    public class ImsxXmlMediaTypeFormatter : XmlMediaTypeFormatter
    {
        // The XSD code generator only creates one imsx_POXEnvelopeType which has the 
        // imsx_POXEnvelopeRequest root element. The IMS spec says the root element
        // should be imsx_POXEnvelopeResponse in the response.

        // Create two serializers: one for requests and one for responses.
        private static readonly XmlSerializer ImsxRequestSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType));
        private static readonly XmlSerializer ImsxResponseSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType), 
            null, null, new XmlRootAttribute("imsx_POXEnvelopeResponse"),
                    "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            try
            {
                SetSerializer<imsx_POXEnvelopeType>(ImsxRequestSerializer);
                return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
            }
            catch (Exception)
            {
                return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
            }
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            try
            {
                SetSerializer<imsx_POXEnvelopeType>(ImsxResponseSerializer);
                return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
            }
            catch (Exception)
            {
                return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
            }
        }
    }
}
