using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti1;
using LtiLibrary.NetCore.Outcomes.v2;
using LtiLibrary.NetCore.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Tests
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
                // Make sure this is an LtiRequest
                try
                {
                    Request.CheckForRequiredLtiParameters();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

                // Parse and validate the request
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
        #region Outcomes-1 (scores)

        /// <summary>
        /// Display the Basic Outcomes settings and provide buttons to exercise the three actions.
        /// </summary>
        /// <param name="lisOutcomeServiceUrl">The URL to the Outcomes service hosted by the Tool Consumer.</param>
        /// <param name="lisResultSourcedId">The SourcedId of the LisResult used in the demo.</param>
        /// <param name="consumerKey">The OAuth Consumer Key to use when sending requests to the Outcomes Service.</param>
        /// <param name="consumerSecret">The OAuth Consumer Secret to use when sending requests to the Outcomes Service.</param>
        /// <remarks>
        /// The Outcomes service is hosted by the Tool Consumer. The Tool Provider call the Outcomes service.
        /// </remarks>
        public ActionResult Outcomes1(string lisOutcomeServiceUrl, string lisResultSourcedId, string consumerKey, string consumerSecret)
        {
            var model = new Outcomes1Model
            {
                LisOutcomeServiceUrl = lisOutcomeServiceUrl,
                LisResultSourcedId = lisResultSourcedId,
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret
            };
            return View(model);
        }

        /// <summary>
        /// Call the Basic Outcomes service on the Tool Consumer.
        /// </summary>
        /// <param name="model">The score to act on.</param>
        /// <param name="submit">The action to take.</param>
        /// <remarks>
        /// The Outcomes service is hosted by the Tool Consumer. The Tool Provider call the Outcomes service.
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult> Outcomes1(Outcomes1Model model, string submit)
        {
            switch (submit)
            {
                case "Send Grade":
                    if (await LtiLibrary.NetCore.Outcomes.v1.OutcomesClient.PostScoreAsync(model.LisOutcomeServiceUrl, model.ConsumerKey, model.ConsumerSecret,
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
                    var lisResult = await LtiLibrary.NetCore.Outcomes.v1.OutcomesClient.ReadScoreAsync(model.LisOutcomeServiceUrl, model.ConsumerKey,
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
                    if (await LtiLibrary.NetCore.Outcomes.v1.OutcomesClient.DeleteScoreAsync(model.LisOutcomeServiceUrl, model.ConsumerKey, model.ConsumerSecret,
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
        public async Task<ActionResult> GetToolConsumerProfile(string url)
        {
            var toolConsumerProfileResponse = await ToolConsumerProfileClient.GetToolConsumerProfileAsync(url);
            return View(toolConsumerProfileResponse);
            //return Content(@"<table><tr><td><pre>" + toolConsumerProfileResponse.HttpRequest+ "</pre>");
        }

        #endregion

        // LTI Outcomes reverse the relationship between Tool Consumers and Tool Providers. The Tool
        // Provider becomes a consumer of the Outcomes service hosted by the Tool Consumer. In this
        // sample, the Tool Provider and tell the Tool Consumer to save, read, and delete scores.
        #region Outcomes-2 (scores)

        /// <summary>
        /// Display the Outcomes V2 settings and provide buttons to exercise the three actions.
        /// </summary>
        public ActionResult Outcomes2(string lineItemServiceUrl, string lineItemsServiceUrl, string resultServiceUrl, string resultsServiceUrl, string contextId, string consumerKey, string consumerSecret)
        {
            var model = new Outcomes2Model
            {
                LineItemServiceUrl = lineItemServiceUrl,
                LineItemsServiceUrl = lineItemsServiceUrl,
                ResultServiceUrl = resultServiceUrl,
                ResultsServiceUrl = resultsServiceUrl,
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                ContextId = contextId
            };
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
        public async Task<ActionResult> Outcomes2(Outcomes2Model model, string submit)
        {
            switch (submit)
            {
                case "Delete LineItem (Delete)":
                    var deleteLineItemResponse = await OutcomesClient.DeleteLineItemAsync(
                        model.LineItemServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = deleteLineItemResponse.HttpRequest;
                    model.HttpResponse = deleteLineItemResponse.HttpResponse;
                    switch (deleteLineItemResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            model.LineItem = null;
                            ModelState.Clear();
                            ViewBag.Message = "200 LineItem deleted";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(deleteLineItemResponse.StatusCode) + " " + deleteLineItemResponse.StatusCode;
                            break;
                    }
                    break;
                case "Get LineItem (Read)":
                    var getLineItemResponse = await OutcomesClient.GetLineItemAsync(
                        model.LineItemServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = getLineItemResponse.HttpRequest;
                    model.HttpResponse = getLineItemResponse.HttpResponse;
                    switch (getLineItemResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            model.LineItem = getLineItemResponse.Outcome;
                            ModelState.Clear();
                            ViewBag.Message = "200 LineItem received";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(getLineItemResponse.StatusCode) + " " + getLineItemResponse.StatusCode;
                            break;
                    }
                    break;
                case "Get LineItemResults (Read)":
                    var getLineItemResultsResponse = await OutcomesClient.GetLineItemWithResultsAsync(
                        model.LineItemServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = getLineItemResultsResponse.HttpRequest;
                    model.HttpResponse = getLineItemResultsResponse.HttpResponse;
                    switch (getLineItemResultsResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            model.LineItem = getLineItemResultsResponse.Outcome;
                            ModelState.Clear();
                            ViewBag.Message = "200 LineItem received";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(getLineItemResultsResponse.StatusCode) + " " + getLineItemResultsResponse.StatusCode;
                            break;
                    }
                    break;
                case "Get LineItems (Read)":
                    var getLineItemsResponse = await OutcomesClient.GetLineItemsAsync(
                        model.LineItemsServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret,
                        activityId: model.LineItem.AssignedActivity.ActivityId);
                    model.HttpRequest = getLineItemsResponse.HttpRequest;
                    model.HttpResponse = getLineItemsResponse.HttpResponse;
                    switch (getLineItemsResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            ViewBag.Message = "200 LineItems received";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(getLineItemsResponse.StatusCode) + " " + getLineItemsResponse.StatusCode;
                            break;
                    }
                    break;
                case "Post LineItem (Create)":
                    var postLineItem = new LineItem
                    {
                        ReportingMethod = "totalScore",
                        LineItemOf = new Context { ContextId = model.ContextId },
                        AssignedActivity = new Activity { ActivityId = model.LineItem.AssignedActivity.ActivityId },
                        ScoreContraints = new NumericLimits { NormalMaximum = 100, ExtraCreditMaximum = 10, TotalMaximum = 110 }
                    };
                    var postLineItemResponse = await OutcomesClient.PostLineItemAsync(
                        postLineItem,
                        model.LineItemsServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = postLineItemResponse.HttpRequest;
                    model.HttpResponse = postLineItemResponse.HttpResponse;
                    switch (postLineItemResponse.StatusCode)
                    {
                        case HttpStatusCode.Created:
                            model.LineItem = postLineItemResponse.Outcome;
                            ModelState.Clear();
                            ViewBag.Message = "201 LineItem added";
                            break;
                        case HttpStatusCode.BadRequest:
                            ViewBag.Message = "400 Bad Request";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(postLineItemResponse.StatusCode) + " " + postLineItemResponse.StatusCode;
                            break;
                    }
                    break;
                case "Put LineItem (Update)":
                    var putLineItem = new LineItem
                    {
                        Id = model.LineItem.Id,
                        ReportingMethod = "totalScore",
                        LineItemOf = new Context { ContextId = model.ContextId },
                        AssignedActivity = new Activity { ActivityId = model.LineItem.AssignedActivity.ActivityId },
                        ScoreContraints = new NumericLimits { NormalMaximum = 100, ExtraCreditMaximum = 10, TotalMaximum = 110 },
                        Results = model.LineItem.Results
                    };
                    var putLineItemResponse = await OutcomesClient.PutLineItemAsync(
                        putLineItem,
                        model.LineItemsServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = putLineItemResponse.HttpRequest;
                    model.HttpResponse = putLineItemResponse.HttpResponse;
                    switch (putLineItemResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            ViewBag.Message = "200 LineItem updated";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(putLineItemResponse.StatusCode) + " " + putLineItemResponse.StatusCode;
                            break;
                    }
                    break;
                case "Delete Result (Delete)":
                    var deleteResultResponse = await OutcomesClient.DeleteResultAsync(
                        model.ResultServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = deleteResultResponse.HttpRequest;
                    model.HttpResponse = deleteResultResponse.HttpResponse;
                    switch (deleteResultResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            model.LineItem = null;
                            ModelState.Clear();
                            ViewBag.Message = "200 Result deleted";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(deleteResultResponse.StatusCode) + " " + deleteResultResponse.StatusCode;
                            break;
                    }
                    break;
                case "Get Result (Read)":
                    var getResultResponse = await OutcomesClient.GetResultAsync(
                        model.ResultServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = getResultResponse.HttpRequest;
                    model.HttpResponse = getResultResponse.HttpResponse;
                    switch (getResultResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            model.Result = getResultResponse.Outcome;
                            ModelState.Clear();
                            ViewBag.Message = "200 Result received";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(getResultResponse.StatusCode) + " " + getResultResponse.StatusCode;
                            break;
                    }
                    break;
                case "Get Results (Read)":
                    var getResultsResponse = await OutcomesClient.GetResultsAsync(
                        model.ResultsServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = getResultsResponse.HttpRequest;
                    model.HttpResponse = getResultsResponse.HttpResponse;
                    switch (getResultsResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            ViewBag.Message = "200 Results received";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(getResultsResponse.StatusCode) + " " + getResultsResponse.StatusCode;
                            break;
                    }
                    break;
                case "Post Result (Create)":
                    var postResult =
                        new LisResult
                        {
                            Comment = "Good job!",
                            ResultAgent = new LisPerson { UserId = "12345" },
                            ResultOf = new Uri(model.LineItemServiceUrl),
                            ResultScore = "0.75",
                            ResultScoreConstraints = new NumericLimits { TotalMaximum = 1 },
                            ResultStatus = ResultStatus.Completed,
                            TotalScore = (decimal?)0.75
                        };
                    var postResultResponse = await OutcomesClient.PostResultAsync(
                        postResult,
                        model.ResultServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = postResultResponse.HttpRequest;
                    model.HttpResponse = postResultResponse.HttpResponse;
                    switch (postResultResponse.StatusCode)
                    {
                        case HttpStatusCode.Created:
                            //model.LineItem = postResult.Outcome;
                            ModelState.Clear();
                            ViewBag.Message = "201 Result added";
                            break;
                        case HttpStatusCode.BadRequest:
                            ViewBag.Message = "400 Bad Request";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(postResultResponse.StatusCode) + " " + postResultResponse.StatusCode;
                            break;
                    }
                    break;
                case "Put Result (Update)":
                    var putResult =
                        new LisResult
                        {
                            Comment = "Good job!",
                            ResultAgent = new LisPerson { UserId = "12345" },
                            ResultOf = new Uri(model.LineItemServiceUrl),
                            ResultScore = "0.75",
                            ResultScoreConstraints = new NumericLimits { TotalMaximum = 1 },
                            ResultStatus = ResultStatus.Final, // Change the status to Final
                            TotalScore = (decimal?)0.75
                        };
                    var putResultResponse = await OutcomesClient.PutResultAsync(
                        putResult,
                        model.ResultServiceUrl,
                        model.ConsumerKey,
                        model.ConsumerSecret);
                    model.HttpRequest = putResultResponse.HttpRequest;
                    model.HttpResponse = putResultResponse.HttpResponse;
                    switch (putResultResponse.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            ViewBag.Message = "200 Result updated";
                            break;
                        case HttpStatusCode.Unauthorized:
                            ViewBag.Message = "401 Not authorized";
                            break;
                        case HttpStatusCode.NotFound:
                            ViewBag.Message = "404 Not found";
                            break;
                        case HttpStatusCode.InternalServerError:
                            ViewBag.Message = "500 Internal server error";
                            break;
                        default:
                            ViewBag.Message = Convert.ToInt32(putResultResponse.StatusCode) + " " + putResultResponse.StatusCode;
                            break;
                    }
                    break;
            }
            return View(model);
        }

        #endregion
    }
}
