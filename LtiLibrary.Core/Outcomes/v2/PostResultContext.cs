using System.Net;

namespace LtiLibrary.Core.Outcomes.v2
{
    public class PostResultContext
    {
        public PostResultContext(LisResult result)
        {
            Result = result;
            StatusCode = HttpStatusCode.OK;
        }

        public LisResult Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
