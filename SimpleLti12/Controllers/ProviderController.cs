using System.IO;
using System.Net;
using LtiLibrary.Common;
using LtiLibrary.Extensions;
using System.Web.Mvc;
using LtiLibrary.Lti1;
using LtiLibrary.Outcomes;
using SimpleLti12.Models;

namespace SimpleLti12.Controllers
{
    public class ProviderController : Controller
    {
        // The most basic function of the Tool Provider is to receive basic launch requests from
        // the Tool Consumer.
        #region LTI 1.0 Tool Provider

        /// <summary>
        /// Display the tool requested by the Tool Consumer.
        /// </summary>
        /// <remarks>
        /// This is the basic function of a Tool Provider.
        /// </remarks>
        public ActionResult Tool()
        {
            try
            {
                // Parse and validate the request
                Request.CheckForRequiredLtiParameters();

                var ltiRequest = new LtiRequest(null);
                ltiRequest.ParseRequest(Request);

                if (!ltiRequest.ConsumerKey.Equals("12345"))
                {
                    ViewBag.Message = "Invalid Consumer Key";
                    return View();
                }

                var oauthSignature = Request.GenerateOAuthSignature("secret");
                if (!oauthSignature.Equals(ltiRequest.Signature))
                {
                    ViewBag.Message = "Invalid Signature";
                    return View();
                }

                // The request is legit, so display the tool
                ViewBag.Message = string.Empty;
                var model = new ToolModel
                {
                    ConsumerSecret = "secret",
                    LtiRequest = ltiRequest
                };
                return View(model);
            }
            catch (LtiException e)
            {
                ViewBag.Message = e.Message;
                return View();
            }
        }

        #endregion

        // LTI Outcomes reverse the relationship between Tool Consumers and Tool Providers. The Tool
        // Provider becomes a consumer of the Outcomes service hosted by the Tool Consumer. In this
        // sample, the Tool Provider and tell the Tool Consumer to save, read, and delete scores.
        #region LTI 1.1 Outcomes (scores)

        /// <summary>
        /// Display the Outcomes settings and provide buttons to exercise the three actions.
        /// </summary>
        /// <param name="lisOutcomeServiceUrl">The URL to the Outcomes service hosted by the Tool Consumer.</param>
        /// <param name="lisResultSourcedId">The SourcedId of the LisResult used in the demo.</param>
        /// <param name="consumerKey">The OAuth Consumer Key to use when sending requests to the Outcomes Service.</param>
        /// <param name="consumerSecret">The OAuth Consumer Secret to use when sending requests to the Outcomes Service.</param>
        /// <remarks>
        /// The Outcomes service is hosted by the Tool Consumer. The Tool Provider call the Outcomes service.
        /// </remarks>
        public ActionResult Outcomes(string lisOutcomeServiceUrl, string lisResultSourcedId, string consumerKey, string consumerSecret)
        {
            var model = new OutcomeModel();
            model.LisOutcomeServiceUrl = lisOutcomeServiceUrl;
            model.LisResultSourcedId = lisResultSourcedId;
            model.ConsumerKey = consumerKey;
            model.ConsumerSecret = consumerSecret;
            return View(model);
        }

        /// <summary>
        /// Call the Outcomes service on the Tool Consumer.
        /// </summary>
        /// <param name="model">The score to act on.</param>
        /// <param name="submit">The action to take.</param>
        /// <remarks>
        /// The Outcomes service is hosted by the Tool Consumer. The Tool Provider call the Outcomes service.
        /// </remarks>
        [HttpPost]
        public ActionResult Outcomes(OutcomeModel model, string submit)
        {
            switch (submit)
            {
                case "Send Grade":
                    if (LtiOutcomesHelper.PostScore(model.LisOutcomeServiceUrl, model.ConsumerKey, model.ConsumerSecret,
                        model.LisResultSourcedId, model.Score))
                    {
                        ViewBag.Message = "Grade sent";
                    }
                    else
                    {
                        ViewBag.Message = "Invalid request";
                    }
                    break;
                case "Read Grade":
                    var lisResult = LtiOutcomesHelper.ReadScore(model.LisOutcomeServiceUrl, model.ConsumerKey,
                        model.ConsumerSecret, model.LisResultSourcedId);
                    if (lisResult.IsValid)
                    {
                        model.Score = lisResult.Score;
                        ViewBag.Message = "Grade read";
                    }
                    else
                    {
                        ViewBag.Message = "No grade";
                    }
                    break;
                case "Delete Grade":
                    if (LtiOutcomesHelper.DeleteScore(model.LisOutcomeServiceUrl, model.ConsumerKey, model.ConsumerSecret,
                        model.LisResultSourcedId))
                    {
                        model.Score = null;
                        ViewBag.Message = "Grade deleted";
                    }
                    else
                    {
                        ViewBag.Message = "Invalid request";
                    }
                    break;
            }
            return View(model);
        }

        #endregion

        // LTI Tool Consumer Profiles allow the Tool Provider to ask the Tool Consumer what 
        // services it provides.
        #region LTI 1.2 Tool Consumer Profile

        /// <summary>
        /// Return a ToolConsumerProfile.
        /// </summary>
        /// <param name="url">The ToolConsumerProfileUrl.</param>
        /// <remarks>
        /// The Tool Provider requests the ToolConsumerProfile. The Tool Consumer responds with a ToolConsumerProfile.
        /// LtiLibrary will return the ToolConsumerProfile in the appropriate format based on the Accept header.
        /// Tool Providers should specify the appropriate Accept header.
        /// </remarks>
        public ActionResult GetToolConsumerProfile(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Accept = LtiConstants.ToolConsumerProfileMediaType;
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var reader = new StreamReader(stream);
                        return Content(reader.ReadToEnd());
                    }
                    return Content("No response.");
                }
            }
        }

        #endregion
    }
}