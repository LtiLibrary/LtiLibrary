using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Membership
{
    /// <summary>
    /// A <see cref="GetMembershipResponse"/> that when executed will produce a Not Found (404) response.
    /// </summary>
    public class NotFoundResponse : GetMembershipResponse
    {
        /// <summary>
        /// Creates a new <see cref="NotFoundResponse"/> instance.
        /// </summary>
        public NotFoundResponse()
        {
            StatusCode = StatusCodes.Status404NotFound;
        }

        /// <summary>
        /// Creates a new <see cref="NotFoundResponse"/> instance.
        /// </summary>
        /// <param name="value">The value to format in the entity body.</param>
        public NotFoundResponse(object value) : this()
        {
            StatusValue = value;
        }
    }
}
