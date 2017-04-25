namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// Represents a DeleteResult DTO.
    /// </summary>
    public class DeleteResultDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public DeleteResultDto(string contextId, string lineItemId, string id)
        {
            ContextId = contextId;
            Id = id;
            LineItemId = lineItemId;
            StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status200OK;
        }

        /// <summary>
        /// Get or set the ContextId.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Get or set the LineItemId.
        /// </summary>
        public string LineItemId { get; set; }

        /// <summary>
        /// Get or set the HTTP StatusCode.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
