using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LtiLibrary.AspNetCore.Tests.ContentItems
{
    public class ContentItemsProviderController : Controller
    {
        /// <summary>
        /// Receives the content item selection request from the Tool Consumer.
        /// </summary>
        /// <remarks>
        /// This is the basic function of a Content-Item Message Provider. Normally
        /// this controller would display some list of possible content. 
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

                // Make sure this is a ContentItemSelectionRequestMessage
                if (!ltiRequest.LtiMessageType.Equals(LtiConstants.ContentItemSelectionRequestLtiMessageType))
                {
                    return BadRequest($"Invalid {LtiConstants.LtiMessageTypeParameter}={ltiRequest.LtiMessageType}");
                }

                // The request is legit
                return Ok(ltiRequest.ToJsonString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Receives the content item selection request from the Tool Consumer
        /// and posts a content item selection.
        /// </summary>
        /// <remarks>
        /// Normally this controller would display some list of possible content,
        /// let the user pick some stuff, then use a separate action to return
        /// the selection to the original requester via the browser. But this controller 
        /// posts a hard coded selection back to the ContentItemReturnUrl.
        /// </remarks>
        public async Task<IActionResult> LibraryThatReturnsSelection()
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

                // Return a list of content items
                var ltiResponse = GetLtiContentItemSelectionResponse(ltiRequest.ContentItemReturnUrl, ltiRequest.Data);
                return Ok(ltiResponse.ToJsonString());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private LtiRequest GetLtiContentItemSelectionResponse(string url, string data)
        {
            // Both links should pass the user's username as a custom parameter
            var custom = new Dictionary<string, string>();
            custom.Add("username", "$User.username");

            // Create a graph with 2 LtiLinks
            var graph = new ContentItemSelectionGraph
            {
                Graph = new List<ContentItem>
                {
                    new LtiLink
                    {
                        Custom = custom,
                        Id = new Uri("https://www.company.com/tool/1"),
                        MediaType = LtiConstants.LtiLtiLinkMediaType,
                        Text = "Tool 1",
                        Title = "Tool 1",
                        PlacementAdvice = new ContentItemPlacement
                        {
                            PresentationDocumentTarget = DocumentTarget.window
                        }
                    },
                    new LtiLink
                    {
                        Custom = custom,
                        Id = new Uri("https://www.company.com/tool/2"),
                        MediaType = LtiConstants.LtiLtiLinkMediaType,
                        Text = "Tool 2",
                        Title = "Tool 2",
                        PlacementAdvice = new ContentItemPlacement
                        {
                            PresentationDocumentTarget = DocumentTarget.iframe
                        }
                    }
                }
            };

            // ReSharper disable once UseObjectOrCollectionInitializer
            var ltiRequest = new LtiRequest(LtiConstants.ContentItemSelectionLtiMessageType)
            {
                ConsumerKey = "12345",
                Url = new Uri(url)
            };
            ltiRequest.ContentItems = graph.ToJsonLdString();
            ltiRequest.Data = data;

            return ltiRequest;
        }
    }
}
