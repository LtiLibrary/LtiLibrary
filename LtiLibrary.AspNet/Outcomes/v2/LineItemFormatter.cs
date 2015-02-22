using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class LineItemFormatter : JsonMediaTypeFormatter
    {
        public LineItemFormatter()
        {
            // Accept LineItem JSON-LD
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(LtiConstants.LineItemMediaType));
        }

        /// <summary>
        /// Return a JsonSerializer that puts @context, @type, and @id before any other
        /// element in an object.
        /// </summary>
        /// <remarks>
        /// This is used by HttpRequestMessage.CreateResponse, which is used by 
        /// ToolConsumerProfileControllerBase to return the profile in the request.
        /// </remarks>
        public override JsonSerializer CreateJsonSerializer()
        {
            return new JsonSerializer
            {
                ContractResolver = new JsonLdObjectContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }
    }
}
