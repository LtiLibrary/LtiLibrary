using LtiLibrary.NetCore.Lti.v1;

namespace LtiLibrary.Canvas.Outcomes.v1
{
    /// <summary>
    /// Represents the read request.
    /// </summary>
    public class ReadResultRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public ReadResultRequest(string lisResultSourcedId)
        {
            LisResultSourcedId = lisResultSourcedId;
        }

        /// <summary>
        /// The LineItemId for this <see cref="Result"/>.
        /// </summary>
        public string LisResultSourcedId { get; set; }
    }
}
