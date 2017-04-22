using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Tool(int id)
        {
            try
            {
                // Parse and validate the request
                var ltiRequest = await Request.ParseLtiRequestAsync();

                // Make sure this is an LtiRequest
                try
                {
                    ltiRequest.CheckForRequiredLtiParameters();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }


                if (!ltiRequest.ConsumerKey.Equals("12345"))
                {
                    return Unauthorized();
                }

                var oauthSignature = ltiRequest.GenerateSignature("secret");
                if (!oauthSignature.Equals(ltiRequest.Signature))
                {
                    return Unauthorized();
                }

                // The request is legit
                return Ok(ltiRequest.ToJsonString());
            }
            catch (LtiException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Display the tool requested by the Tool Consumer.
        /// </summary>
        /// <remarks>
        /// This is the basic function of a Tool Provider.
        /// </remarks>
        public async Task<IActionResult> Library()
        {
            try
            {
                // Parse and validate the request
                var ltiRequest = await Request.ParseLtiRequestAsync();

                // Make sure this is an LtiRequest
                try
                {
                    ltiRequest.CheckForRequiredLtiParameters();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

                if (!ltiRequest.ConsumerKey.Equals("12345"))
                {
                    return Unauthorized();
                }

                var oauthSignature = ltiRequest.GenerateSignature("secret");
                if (!oauthSignature.Equals(ltiRequest.Signature))
                {
                    return Unauthorized();
                }

                // The request is legit
                return Ok(ltiRequest.ToJsonString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
