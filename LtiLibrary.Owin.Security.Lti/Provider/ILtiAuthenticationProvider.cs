using System.Threading.Tasks;

namespace LtiLibrary.Owin.Security.Lti.Provider
{
    public interface ILtiAuthenticationProvider
    {
        /// <summary>
        /// Invoked before the LTI request has been authenticated so the application can supply the appropriate OAuth secret
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the LTI request.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task Authenticate(LtiAuthenticateContext context);

        /// <summary>
        /// Invoked after the LTI request has been authenticated so the application can sign in the application user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the LTI request.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task Authenticated(LtiAuthenticatedContext context);

        /// <summary>
        /// Invoked by the application if a username must be generated for the LTI user
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the LTI request.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        Task GenerateUserName(LtiGenerateUserNameContext context);
    }
}
