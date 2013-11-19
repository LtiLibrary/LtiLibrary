using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Provider.Models;
using WebMatrix.WebData;

namespace Provider.Lti
{
    public static class LtiSecurity
    {
        public static ICollection<LtiLogin> GetLtiLoginsForUser(int userId)
        {
            using (var db = new ProviderContext())
            {
                var pairedUsers = db.PairedUsers.Where(u => u.User.UserId == userId).ToList();
                var ltiLogins = new List<LtiLogin>();
                foreach (var pairedUser in pairedUsers)
                {
                    var consumer = db.Consumers.Find(pairedUser.ConsumerId);
                    ltiLogins.Add(new LtiLogin
                    {
                        ConsumserId = pairedUser.ConsumerId,
                        ConsumerName = consumer.Name,
                        ConsumerUserId = pairedUser.ConsumerUserId
                    });
                }
                return ltiLogins;
            }
        }

        public static bool HasLocalAccount(int userId)
        {
            var provider = Membership.Provider as ExtendedMembershipProvider;
            return provider != null && provider.HasLocalAccount(userId);
        }

        /// <summary>
        /// Get a value indicating whether the current user is a proxy for
        /// a Tool Consumer user.
        /// </summary>
        public static bool IsLtiUser
        {
            get
            {
                var formsIdentity = HttpContext.Current.User.Identity as FormsIdentity;
                if (formsIdentity == null)
                {
                    return false;
                }
                var userData = formsIdentity.Ticket.UserData;
                if (String.IsNullOrEmpty(userData))
                {
                    return false;
                }
                return userData.StartsWith("LTI");
            }
        }

        /// <summary>
        /// Login the local User account for the LTI user.
        /// </summary>
        /// <param name="pairedUser">The local account paired to the incoming LTI user.</param>
        /// <param name="createPersistentCookie"></param>
        public static void Login(PairedUser pairedUser, bool createPersistentCookie = false)
        {
            // This FormsAuthentication cookie tells the website that
            // the consumer user is now logged in. The custom userData
            // holds the Tool Consumer ConsumerUser information so that
            // providers can look at things like LTI roles.
            var userData = String.Format("LTI,{0},{1}",
                pairedUser.ConsumerId, pairedUser.ConsumerUserId);
            var authTicket = new FormsAuthenticationTicket(
                1, // version
                pairedUser.User.UserName,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                createPersistentCookie,
                userData,
                FormsAuthentication.FormsCookiePath);
            var encTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
            {
                HttpOnly = true,
                Path = FormsAuthentication.FormsCookiePath
            };
            HttpContext.Current.Response.Cookies.Add(authCookie);

            // Add authentication to current request
            var id = new FormsIdentity(authTicket);
            var principal = new GenericPrincipal(id, new string[] { });
            HttpContext.Current.User = principal;
        }
    }
}