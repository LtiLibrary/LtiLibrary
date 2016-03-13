using System.Net;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class DeleteResultContext
    {
        public DeleteResultContext(string id)
        {
            Id = id;
            StatusCode = HttpStatusCode.OK;
        }

        public string Id { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
