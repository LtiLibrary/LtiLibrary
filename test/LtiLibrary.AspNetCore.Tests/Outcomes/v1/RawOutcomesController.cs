using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v1
{
    /// <summary>
    /// This version of the OutcomesController does not use model binding so that
    /// it can inspect the raw data in the request.
    /// </summary>
    [Consumes("application/xml")]
    [Produces("text/plain")]
    [Route("ims/rawoutcomes", Name = "RawOutcomesApi")]
    public class RawOutcomesController : Controller
    {
        private static readonly XmlSerializer ImsxResponseSerializer = new XmlSerializer(typeof(imsx_POXEnvelopeType),
            null, null, new XmlRootAttribute("imsx_POXEnvelopeResponse"),
            "http://www.imsglobal.org/services/ltiv1p1/xsd/imsoms_v1p0");

        /// <summary>
        /// All Outcomes1Client requests are handled by this one action
        /// </summary>
        public async Task<IActionResult> Post()
        {
            var contentEncoding = GetContentEncoding(Request.ContentType);

            if (Request.Body.CanSeek)
            {
                Request.Body.Position = 0;
            }
            if (Request.Body.CanRead)
            {
                var reader = new StreamReader(Request.Body);
                var xml = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(xml))
                {
                    return BadRequest();
                }

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var declaration = doc.FirstChild as XmlDeclaration;
                if (declaration == null)
                {
                    return BadRequest();
                }

                if (!contentEncoding.Equals(declaration.Encoding))
                {
                    return BadRequest();
                }
            }
            return await CreateSuccessResponse();
        }

        /// <summary>
        /// Create a simple, but complete response envelope. The status is set to success.
        /// </summary>
        private static async Task<ObjectResult> CreateSuccessResponse()
        {
            var response = new imsx_POXEnvelopeType
            {
                imsx_POXHeader = new imsx_POXHeaderType { Item = new imsx_ResponseHeaderInfoType() },
                imsx_POXBody = new imsx_POXBodyType { Item = new readResultResponse() }
            };

            var item = (imsx_ResponseHeaderInfoType)response.imsx_POXHeader.Item;
            item.imsx_version = imsx_GWSVersionValueType.V10;
            item.imsx_messageIdentifier = Guid.NewGuid().ToString();
            item.imsx_statusInfo = new imsx_StatusInfoType();

            var status = item.imsx_statusInfo;
            status.imsx_codeMajor = imsx_CodeMajorType.success;
            status.imsx_severity = imsx_SeverityType.status;

            using (var ms = new MemoryStream())
            {
                ImsxResponseSerializer.Serialize(ms, response);
                using (var reader = new StreamReader(ms))
                {
                    ms.Position = 0;
                    return new ObjectResult(await reader.ReadToEndAsync());
                }
            }
        }

        /// <summary>
        /// Get the encoding from the Content-Type header.
        /// </summary>
        private static string GetContentEncoding(string contentType)
        {
            if (!contentType.Contains("charset="))
            {
                return "utf-8";
            }

            var index = contentType.IndexOf("charset=", StringComparison.Ordinal);
            var encoding = contentType.Substring(index + "charset=".Length);
            if (encoding.Contains(";"))
            {
                encoding = encoding.Substring(0, encoding.IndexOf(";", StringComparison.Ordinal));
            }
            return encoding;
        }
    }
}
