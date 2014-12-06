namespace Provider.Owin.Provider
{
    public interface ILtiAuthenticationProvider
    {
        /// <summary>
        /// Called when an exception occurs during request or response processing.
        /// </summary>
        /// <param name="context">Contains information about the exception that occurred</param>
        void Exception(LtiAuthenticationExceptionContext context);
    }
}