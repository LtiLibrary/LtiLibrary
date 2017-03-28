namespace LtiLibrary.AspNet.Outcomes.v2
{
    public class DeleteResultContext
    {
        public DeleteResultContext(string contextId, string lineItemId, string id)
        {
            ContextId = contextId;
            Id = id;
            LineItemId = lineItemId;
            StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status200OK;
        }

        public string ContextId { get; set; }
        public string Id { get; set; }
        public string LineItemId { get; set; }
        public int StatusCode { get; set; }
    }
}
