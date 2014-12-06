using System;
using Owin;

namespace LtiLibrary.Owin.Security.Lti
{
    /// <summary>
    /// Extension methods for using <see cref="LtiAuthenticationMiddleware"/>
    /// </summary>
    public static class LtiAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using LTI
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseLtiAuthentication(this IAppBuilder app, LtiAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            return app.Use(typeof(LtiAuthenticationMiddleware), app, options);
        }
    }
}
