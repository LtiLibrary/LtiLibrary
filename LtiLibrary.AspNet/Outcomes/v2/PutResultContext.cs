using System.Net;
using LtiLibrary.Core.Outcomes.v2;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class PutResultContext
    {
        public PutResultContext(LisResult result)
        {
            Result = result;
            StatusCode = HttpStatusCode.OK;
        }

        public LisResult Result { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
