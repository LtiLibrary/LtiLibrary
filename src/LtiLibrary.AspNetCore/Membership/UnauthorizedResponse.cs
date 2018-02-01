using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Membership
{
    /// <summary>
    /// A <see cref="GetMembershipResponse"/> that when executed will produce a Unauthorized (401) response.
    /// </summary>
    public class UnauthorizedResponse : GetMembershipResponse
    {
        /// <summary>
        /// Creates a new <see cref="UnauthorizedResponse"/> instance.
        /// </summary>
        public UnauthorizedResponse()
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }

        /// <summary>
        /// Creates a new <see cref="UnauthorizedResponse"/> instance.
        /// </summary>
        /// <param name="value">The value to format in the entity body.</param>
        public UnauthorizedResponse(object value) : this()
        {
            StatusValue = value;
        }
    }
}
