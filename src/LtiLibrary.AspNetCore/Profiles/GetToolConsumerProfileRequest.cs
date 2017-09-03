namespace LtiLibrary.AspNetCore.Profiles
{
    /// <summary>
    /// Represents a GetToolConsumerProfile request.
    /// </summary>
    public class GetToolConsumerProfileRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetToolConsumerProfileRequest(string ltiVersion)
        {
            LtiVersion = ltiVersion;
        }

        /// <summary>
        /// Get or set the LtiVersion.
        /// </summary>
        public string LtiVersion { get; }
    }
}
