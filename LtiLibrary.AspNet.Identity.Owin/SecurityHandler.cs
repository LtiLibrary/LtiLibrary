using System;
using System.Collections.Generic;
using System.Security.Claims;
using LtiLibrary.Owin.Security.Lti.Provider;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace LtiLibrary.AspNet.Identity.Owin
{
    public static class SecurityHandler
    {
        /// <summary>
        /// Invoked after the LTI request has been authenticated so the application can sign in the application user.
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the LTI request.</param>
        /// <param name="claims">Optional set of claims to add to the identity.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public static async Task OnAuthenticated<TManager, TUser>(LtiAuthenticatedContext context, IEnumerable<Claim> claims = null) 
            where TManager : UserManager<TUser, string> 
            where TUser : IdentityUser, new()
        {
            // Find existing pairing between LTI user and application user
            var userManager = context.OwinContext.GetUserManager<TManager>();
            var loginProvider = string.Join(":", new [] { context.Options.AuthenticationType, context.LtiRequest.ConsumerKey });
            var providerKey = context.LtiRequest.UserId;
            var login = new UserLoginInfo(loginProvider, providerKey);
            var user = await userManager.FindAsync(login);
            if (user == null)
            {
                var usernameContext = new LtiGenerateUserNameContext(context.OwinContext, context.LtiRequest);
                await context.Options.Provider.GenerateUserName(usernameContext);
                if (string.IsNullOrEmpty(usernameContext.UserName))
                {
                    return;
                }
                user = await userManager.FindByNameAsync(usernameContext.UserName);
                if (user == null)
                {
                    user = new TUser {UserName = usernameContext.UserName, Email = usernameContext.UserName};
                    var result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return;
                    }
                }
                // Save the pairing between LTI user and application user
                await userManager.AddLoginAsync(user.Id, login);
            }

            // Create the application identity, add the LTI request as a claim, and sign in
            var identity = await userManager.CreateIdentityAsync(user, context.Options.SignInAsAuthenticationType);
            identity.AddClaim(new Claim(
                type: context.Options.ClaimType, 
                value: JsonConvert.SerializeObject(context.LtiRequest, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                valueType: ClaimValueTypes.String,
                issuer: context.Options.AuthenticationType));
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    identity.AddClaim(claim);
                }
            }
            context.OwinContext.Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
        }

        /// <summary>
        /// Generate a valid application username using information from an LTI request. The default
        /// ASP.NET application using Microsoft Identity uses an email address as the username. This
        /// code will generate an "anonymous" email address if one is not supplied in the LTI request.
        /// </summary>
        /// <param name="context">Contains information about the login session as well as the LTI request.</param>
        /// <returns>A <see cref="Task"/> representing the completed operation.</returns>
        public static Task OnGenerateUserName(LtiGenerateUserNameContext context)
        {
            if (string.IsNullOrEmpty(context.LtiRequest.LisPersonEmailPrimary))
            {
                var username = string.Concat("anon-", context.LtiRequest.UserId);
                Uri url;
                if (string.IsNullOrEmpty(context.LtiRequest.ToolConsumerInstanceUrl) 
                    || !Uri.TryCreate(context.LtiRequest.ToolConsumerInstanceUrl, UriKind.Absolute, out url))
                {
                    context.UserName = string.Concat(username, "@anon-", context.LtiRequest.ConsumerKey, ".lti");
                }
                else
                {
                    context.UserName = string.Concat(username, "@", url.Host);
                }
            }
            else
            {
                context.UserName = context.LtiRequest.LisPersonEmailPrimary;
            }

            return Task.FromResult<object>(null);
        }
    }
}
