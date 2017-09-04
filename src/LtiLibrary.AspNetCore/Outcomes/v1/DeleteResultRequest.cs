using LtiLibrary.NetCore.Lti.v1;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// Represents the delete request.
    /// </summary>
    public class DeleteResultRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public DeleteResultRequest(string lisResultSourcedId)
        {
            LisResultSourcedId = lisResultSourcedId;
        }

        /// <summary>
        /// The LineItemId for this <see cref="Result"/>.
        /// </summary>
        public string LisResultSourcedId { get; set; }
    }
}
