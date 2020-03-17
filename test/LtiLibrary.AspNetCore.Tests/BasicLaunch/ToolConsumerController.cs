using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Tests.BasicLaunch
{
    public class ToolConsumerController : Controller
    {
        /// <summary>
        /// Return a basic launch form that contains a value that should be encoded
        /// </summary>
        public async void Launch()
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                ConsumerKey = "12345",
                LisPersonNameFull = "d'Artagnan",
                Nonce = "f68bdf5dbed34d7aa6ff18456bf3185e",
                ResourceLinkId = "link",
                Timestamp = 1517630398,
                Url = Request.GetUri(),
                LaunchPresentationLocale = "en-US"
            };
            await Response.WriteLtiRequest(request, "secret");
        }
    }
}
