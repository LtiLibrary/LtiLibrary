using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    public class DeleteLineItemDto
    {
        public DeleteLineItemDto(string contextId, string id)
        {
            ContextId = contextId;
            Id = id;
            StatusCode = StatusCodes.Status200OK;
        }

        public string ContextId { get; set; }
        public string Id { get; }
        public int StatusCode { get; set; }
    }
}
