using LtiLibrary.Common;
using LtiLibrary.ContentItems;
using LtiLibrary.Lti1;
using LtiLibrary.Outcomes;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Provider.Filters;
using Provider.Lti;
using Provider.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Provider.Controllers
{
    [XFrameOptionsHeader]
    public class ToolController : Controller
    {
        public ToolController() {}

        public ToolController(ProviderContext providerContext)
        {
            ProviderContext = providerContext;
        }

        private ProviderContext _providerContext;
        private ProviderContext ProviderContext
        {
            get
            {
                return _providerContext ?? HttpContext.GetOwinContext().Get<ProviderContext>();
            }
            set
            {
                _providerContext = value;
            }
        }

        /// <summary>
        /// Display LTI launch context
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        [Authorize]
        public ActionResult Context()
        {
            var ltiRequest = GetLtiRequestFromClaim();
            var consumer = ProviderContext.Consumers.SingleOrDefault(c => c.Key.Equals(ltiRequest.ConsumerKey));
            if (consumer == null) return null;

            var toolUser = new ToolUser
            {
                ConsumerName = consumer.Name,
                FirstName = ltiRequest.LisPersonNameGiven,
                LastName = ltiRequest.LisPersonNameFamily,
                ReturnUrl = ltiRequest.LaunchPresentationReturnUrl,
                Roles = ltiRequest.Roles
            };
            return PartialView("_ContextPartial", toolUser);
        }

        //
        // GET: /Tool/Create
        [Authorize]
        public ActionResult Create()
        {
            var model = new Tool();
            return View(model);
        }

        //
        // POST: /Tool/Create
        [HttpPost]
        [ValidateInput(false)]
        [Authorize]
        public ActionResult Create(Tool tool)
        {
            if (ModelState.IsValid)
            {
                ProviderContext.Tools.Add(tool);
                ProviderContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tool);
        }

        //
        // GET: /Tool/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            Tool tool = ProviderContext.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        //
        // POST: /Tool/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Tool tool = ProviderContext.Tools.Find(id);
            ProviderContext.Tools.Remove(tool);
            ProviderContext.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Tool/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            var tool = ProviderContext.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        //
        // GET: /Tool/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            var tool = ProviderContext.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        //
        // POST: /Tool/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        [Authorize]
        public ActionResult Edit(Tool tool)
        {
            if (ModelState.IsValid)
            {
                ProviderContext.Entry(tool).State = EntityState.Modified;
                ProviderContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tool);
        }

        private Outcome GetOutcomeFromClaim()
        {
            var user = User.Identity as ClaimsIdentity;
            if (user == null) return null;

            var claim = user.Claims.SingleOrDefault(c => c.Type.Equals("OutcomeId"));
            if (claim == null) return null;

            int outcomeId;
            return int.TryParse(claim.Value, out outcomeId)
                ? ProviderContext.Outcomes.Find(outcomeId)
                : null;
        }

        private LtiRequest GetLtiRequestFromClaim()
        {
            var user = User.Identity as ClaimsIdentity;
            if (user == null) return null;

            var claim = user.Claims.SingleOrDefault(c => c.Type.Equals("ProviderRequestId"));
            if (claim == null) return null;

            int providerRequestId;
            if (!int.TryParse(claim.Value, out providerRequestId)) return null;
            
            var providerRequest = ProviderContext.ProviderRequests.Find(providerRequestId);
            if (providerRequest == null) return null;

            return JsonConvert.DeserializeObject<LtiRequest>(providerRequest.LtiRequest);
        }

        //
        // GET: /Tool/
        [Authorize]
        public ActionResult Index()
        {
            return View(ProviderContext.Tools.ToList());
        }

        [Authorize]
        public ActionResult PlaceContentItem(int id)
        {
            var tool = ProviderContext.Tools.Find(id);
            if (tool == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid tool id" });
            }

            var ltiRequest = GetLtiRequestFromClaim();
            if (ltiRequest == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid LTI request" });
            }

            var consumer = ProviderContext.Consumers.SingleOrDefault(c => c.Key.Equals(ltiRequest.ConsumerKey));
            if (consumer == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid consumer" });
            }

            // Prepare the custom parameters this TP would like on each link.
            var custom = new Dictionary<string, string>();
            // The next two custom parameters use well-known custom parameter substitution variables.
            custom.Add("username", "$User.username"); // Used by this TP when pairing a new user
            custom.Add("tc_profile_url", "$ToolConsumerProfile.url"); // Used by this TP to determine TC capabilities
            // The next three custom parameters use custom, custom parameter substitution variables.
            // If the TC supports them, GREAT. If not this TP will gracefully degrade.
            custom.Add("state_id", "$Context.stateId"); // State abbreviation (i.e. "AK" or "OR")
            custom.Add("district_id", "$Context.ncesLeaId"); // NCES id for the district (LEA)
            custom.Add("school_id", "$Context.ncesSchoolId"); // NCES id for the school

            // Determine the best PresentationDocumentTarget from the list of targets acceptable
            // to the TC assuming the TC sent the list of acceptable targets in priority order
            var presentationDocumentTarget = DocumentTarget.iframe;
            var acceptablePresentationDocumentTargets =
                ParseDocumentTargets(ltiRequest.AcceptPresentationDocumentTargets);
            if (acceptablePresentationDocumentTargets.Count > 0)
            {
                if (!acceptablePresentationDocumentTargets.Contains(presentationDocumentTarget))
                {
                    presentationDocumentTarget = acceptablePresentationDocumentTargets[0];
                }
            }

            // Calculate the full qualified URL for the tool.
            var toolUrl = UrlHelper.GenerateUrl("Default", "View", "Tool", new RouteValueDictionary(new { id }), 
                RouteTable.Routes, Request.RequestContext, false);
            Uri toolUri;
            if (Uri.TryCreate(Request.Url, toolUrl, out toolUri))
            {
                toolUrl = toolUri.AbsoluteUri;
            }

            // Start building the response
            var graph = new List<ContentItemPlacement>
            {
                new ContentItemPlacement
                {
                    PresentationDocumentTarget = presentationDocumentTarget,
                    PlacementOf = new ContentItem(ContentItemType.LtiLink)
                    {
                        Custom = custom,
                        Id = toolUrl,
                        MediaType = LtiConstants.LaunchMediaType,
                        Text = tool.Description ?? ltiRequest.Text,
                        Title = tool.Name ?? ltiRequest.Title
                    }
                }
            };
            var response = new ContentItemPlacementResponse
            {
                Graph = graph
            };

            // Content-Item Message 1.0 does not include a ContentItemService. This is
            // a placeholder for experimenting.
            if (!string.IsNullOrEmpty(ltiRequest.ContentItemServiceUrl))
            {
                var success = ContentItemsHelper.PostContentItems(
                    ltiRequest.ContentItemServiceUrl, consumer.Key,
                    consumer.Secret, response, ltiRequest.Data);

                return RedirectToAction("Search", new { success });
            }

            // Content-Item Message 1.0 sends each request (which can have many items)
            // back to the Tool Consumer.
            var model = ContentItemsHelper.CreateContentItemSelectionResponseViewModel(
                ltiRequest.ContentItemReturnUrl, consumer.Key,
                consumer.Secret, response, ltiRequest.Data,
                null, null, null, "Selected " + tool.Name);
            return View(model);
        }

        private IList<DocumentTarget> ParseDocumentTargets(string documentTargets)
        {
            var targets = new List<DocumentTarget>();
            if (string.IsNullOrWhiteSpace(documentTargets)) return targets;

            foreach (var targetName in documentTargets.Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                DocumentTarget target;
                if (Enum.TryParse(targetName, out target))
                {
                    targets.Add(target);
                }
            }
            return targets;
        }

        [ChildActionOnly]
        [Authorize]
        public ActionResult PostScores(int id)
        {
            var outcome = GetOutcomeFromClaim();
            if (outcome != null)
            {
                var consumer = ProviderContext.Consumers.Find(outcome.ConsumerId);
                var score = LtiOutcomesHelper.ReadScore(outcome.ServiceUrl, consumer.Key, consumer.Secret, outcome.LisResultSourcedId);
                if (score.IsValid)
                {
                    return PartialView("_PostScoresPartial",
                        new PostScoreModel
                        {
                            ConsumerName = consumer.Name,
                            ContextTitle = outcome.ContextTitle,
                            OutcomeId = outcome.OutcomeId,
                            Score = score.Score,
                            ToolId = id
                        });
                }
            }
            return new EmptyResult();
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostScore(PostScoreModel model)
        {
            var outcome = ProviderContext.Outcomes.Find(model.OutcomeId);
            var consumer = ProviderContext.Consumers.Find(outcome.ConsumerId);
            var success  = LtiOutcomesHelper.PostScore(
                outcome.ServiceUrl, consumer.Key, consumer.Secret, outcome.LisResultSourcedId, model.Score);
            return RedirectToAction("View", new { id = model.ToolId, success });
        }

        //
        // GET: /Tool/
        [Authorize]
        public ActionResult Search(bool? success)
        {
            // If the tool is being redisplayed after sending an assignment, then display the result.
            if (success.HasValue)
            {
                ViewBag.StatusMessage = success.Value ? "The assignment was sent." : "The assignment was not sent.";
            }

            return View(ProviderContext.Tools.OrderBy(t => t.Name).ToList());
        }

        //
        // GET: //Tool/Unauthorized/5
        [AllowAnonymous]
        public ActionResult Unauthorized(int id = 0)
        {
            var tool = ProviderContext.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        //
        // GET: /Tool/View/5
        //
        // The 2 Authorize filters force the current User
        // to be authenticated and authorized to access this
        // action. The final authorization step is to
        // make sure this user is authorized to see this 
        // specific tool.
        [Authorize]
        public ActionResult View(int id, bool? success)
        {
            var tool = ProviderContext.Tools.Find(id);
            if (tool == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid tool id" });
            }

            var ltiRequest = GetLtiRequestFromClaim();
            if (ltiRequest == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid LTI request" });
            }

            // If this is a content-item message, display the the appropriate actions
            ViewBag.ShowAssign = ltiRequest.LtiMessageType.Equals(LtiConstants.ContentItemSelectionRequestLtiMessageType,
                StringComparison.OrdinalIgnoreCase);

            // If there is a possible outcome associated with this request,
            // add it to the tool
            var outcome = GetOutcomeFromClaim();
            if (outcome != null)
            {
                if (tool.Outcomes.All(o => o.OutcomeId != outcome.OutcomeId))
                {
                    tool.Outcomes.Add(outcome);
                    ProviderContext.SaveChanges();
                }
            }

            // If the tool is being redisplayed after posting a score,
            // then display the result.
            if (success.HasValue)
            {
                ViewBag.StatusMessage = success.Value ? "The score was sent" : "The score was not sent.";
            }

            return View(tool);
        }
    }
}