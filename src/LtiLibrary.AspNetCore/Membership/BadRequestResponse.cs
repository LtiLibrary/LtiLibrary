using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Membership
{
    /// <summary>
    /// A <see cref="GetMembershipResponse"/> that when executed will produce a Bad Request (400) response.
    /// </summary>
    public class BadRequestResponse : GetMembershipResponse
    {
        /// <summary>
        /// Creates a new <see cref="BadRequestResponse"/> instance.
        /// </summary>
        public BadRequestResponse()
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }

        /// <summary>
        /// Creates a new <see cref="BadRequestResponse"/> instance.
        /// </summary>
        /// <param name="error">Contains the errors to be returned to the client.</param>
        public BadRequestResponse(object error) : this()
        {
            StatusValue = error;
        }
    }
}
