using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LtiLibrary.Provider;
using Provider.Extensions;
using Provider.Lti;
using Provider.Models;

namespace Provider.Controllers
{
    public class ToolController : Controller
    {
        private readonly ProviderContext _db = new ProviderContext();

        private object LtiInboundRequestId
        {
            get { return TempData["LtiInboundRequestId"]; }
            set { TempData["LtiInboundRequestId"] = value; }
        }

        //
        // GET: /Tool/

        public ActionResult Index()
        {
            return View(_db.Tools.ToList());
        }

        //
        // GET: /Tool/Details/5
        // POST: /Tool/Details/5
        
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            Tool tool = _db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
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
        [Authorize]
        [ValidateInput(false)]
        public ActionResult Create(Tool tool)
        {
            if (ModelState.IsValid)
            {
                _db.Tools.Add(tool);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tool);
        }

        //
        // GET: /Tool/Edit/5

        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            Tool tool = _db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        //
        // POST: /Tool/Edit/5

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult Edit(Tool tool)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(tool).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tool);
        }

        //
        // GET: /Tool/Delete/5

        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            Tool tool = _db.Tools.Find(id);
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
            Tool tool = _db.Tools.Find(id);
            _db.Tools.Remove(tool);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: //Tool/Unauthorized/5

        public ActionResult Unauthorized(int id = 0)
        {
            Tool tool = _db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        //
        // GET: /Tool/View/5

        // The 2 Authorize filters force the current User
        // to be authenticated and authorized to access this
        // action. The final authorization step is to
        // make sure this user is authorized to see this 
        // specific tool.

        [LtiAuthorize(Order = 0)]
        [Authorize(Order = 1)]
        public ActionResult View(int id, ViewMessageId? message, int? ltiInboundRequestId)
        {
            var tool = _db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }

            // Capture the LtiInboundRequestId so that it is available
            // to all the child actions and on redirects
            if (ltiInboundRequestId.HasValue)
            {
                LtiInboundRequestId = ltiInboundRequestId.Value;
            }

            // If there is a possible outcome associated with this request,
            // add it to the tool

            var ltiRequest = _db.LtiRequests.Find(LtiInboundRequestId);
            var outcome = _db.Outcomes.Find(ltiRequest.OutcomeId);
            if (outcome != null)
            {
                if (tool.Outcomes.All(o => o.OutcomeId != outcome.OutcomeId))
                {
                    tool.Outcomes.Add(outcome);
                    _db.SaveChanges();
                }
            }

            // If the tool is being redisplayed after posting a score,
            // then display the result.

            ViewBag.StatusMessage =
                message == ViewMessageId.PostScoreFailure ? "The score was not sent."
                : message == ViewMessageId.PostScoreSuccess ? "The score was sent."
                : "";

            return View(tool);

        }

        [ChildActionOnly]
        public ActionResult PostScores(int toolId)
        {
            var tool = _db.Tools.Find(toolId);
            var postScores = new List<PostScoreModel>();
            foreach (var outcome in tool.Outcomes.ToList())
            {
                var score = LtiOutcomesHandler.ReadScore(outcome.OutcomeId);
                if (score.IsValid)
                {
                    var consumer = _db.Consumers.Find(outcome.ConsumerId);
                    postScores.Add(new PostScoreModel
                    {
                        ConsumerName = consumer.Name,
                        ContextTitle = outcome.ContextTitle,
                        LtiInboundRequestId = (int) LtiInboundRequestId,
                        OutcomeId = outcome.OutcomeId,
                        Score = score.Score,
                        ToolId = toolId
                    });
                }
            }
            return PartialView("_PostScoresPartial", postScores);
        }

        [HttpPost]
        public ActionResult PostScore(PostScoreModel model)
        {
            ViewMessageId? message = LtiOutcomesHandler.PostScore(model.OutcomeId, model.Score) 
                                         ? ViewMessageId.PostScoreSuccess : ViewMessageId.PostScoreFailure;
            return RedirectToAction("View", new { id = model.ToolId, Message = message, model.LtiInboundRequestId });
        }

        [ChildActionOnly]
        public ActionResult Context()
        {
            var ltiRequest = _db.LtiRequests.Find(LtiInboundRequestId);
            var consumer = _db.Consumers.Find(ltiRequest.ConsumerId);
            var toolUser = new ToolUser
            {
                ConsumerName = consumer.Name,
                DistrictName = ltiRequest.GetDistrictName(),
                FirstName = ltiRequest.LisPersonNameGiven,
                LastName = ltiRequest.LisPersonNameFamily,
                ReturnUrl = ltiRequest.LaunchPresentationReturnUrl,
                Roles = ltiRequest.RolesAsString,
                SchoolName = ltiRequest.GetSchoolName(),
                StateName = ltiRequest.GetStateName()
            };
            return PartialView("_ContextPartial", toolUser);
        }

        public enum ViewMessageId
        {
            PostScoreSuccess,
            PostScoreFailure,
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}