using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti1;
using LtiLibrary.NetCore.Outcomes.v2;
using LtiLibrary.NetCore.Profiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Filter;

namespace LtiLibrary.AspNetCore.Tests.Lti1
{
    public class ProviderController : Controller
    {
        /// <summary>
        /// Display the tool requested by the Tool Consumer.
        /// </summary>
        /// <remarks>
        /// This is the basic function of a Tool Provider.
        /// </remarks>
        public async Task<ActionResult> Tool()
        {
            try
            {
                // Make sure this is an LtiRequest
                try
                {
                    Request.CheckForRequiredLtiFormParameters();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

                // Parse and validate the request
                var ltiRequest = await Request.ParseLtiRequestAsync();

                if (!ltiRequest.ConsumerKey.Equals("12345"))
                {
                    return BadRequest("Invalid Consumer Key");
                }

                var oauthSignature = ltiRequest.GenerateSignature("secret");
                if (!oauthSignature.Equals(ltiRequest.Signature))
                {
                    return BadRequest("Invalid Signature");
                }

                // The request is legit, so display the data
                Debug.WriteLine($"{ltiRequest.HttpMethod} {ltiRequest.Url}");
                foreach (string key in ltiRequest.Parameters.AllKeys)
                {
                    Debug.WriteLine($"{key}={ltiRequest.Parameters[key]}");
                }
                return Ok();
            }
            catch (LtiException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
