using System.Net.Http.Headers;

namespace LtiLibrary.NetCore.Extensions
{
    public static class HttpContentType
    {
        public static readonly MediaTypeHeaderValue Xml = new MediaTypeHeaderValue("application/xml");
    }
}
