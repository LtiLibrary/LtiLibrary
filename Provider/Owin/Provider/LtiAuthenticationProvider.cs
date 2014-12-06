using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Provider.Owin.Provider
{
    public class LtiAuthenticationProvider : ILtiAuthenticationProvider
    {
        public LtiAuthenticationProvider()
        {
            OnException = context => { };
        }

        /// <summary>
        /// A delegate assigned to this property will be invoked when the related method is called
        /// </summary>
        public Action<LtiAuthenticationExceptionContext> OnException { get; set; }

        public void Exception(LtiAuthenticationExceptionContext context)
        {
            OnException.Invoke(context);
        }
    }
}