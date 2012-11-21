using System.Data;
using System.Linq;
using System.Web.Mvc;
using Consumer.Models;
using Consumer.Utility;
using WebMatrix.WebData;

namespace Consumer.Controllers
{
    public class AssignmentController : Controller
    {
        private ConsumerContext db = new ConsumerContext();

        //
        // GET: /Assignment/

        public ActionResult Index()
        {
            return View(db.Assignments.ToList());
        }

        //
        // GET: /Assignment/Details/5

        public ActionResult Details(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        //
        // GET: /Assignment/Create

        [Authorize(Roles="Teacher")]
        public ActionResult Create()
        {
            ViewBag.LtiVersionId = new SelectList(db.LtiVersions, "LtiVersionId", "Name", LtiVersion.Version10);
            ViewBag.SharingScopeId = new SelectList(db.SharingScopes, "SharingScopeId", "Name", SharingScope.Private);
            return View();
        }

        //
        // POST: /Assignment/Create
        //
        // Note that ValidateInput is turned off so that teachers can use HTML in their descriptions

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateInput(false)]
        public ActionResult Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.UserId = WebSecurity.CurrentUserId;
                db.Assignments.Add(assignment);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.LtiVersionId = new SelectList(db.LtiVersions, "LtiVersionId", "Name", assignment.LtiVersionId);
            ViewBag.SharingScopeId = new SelectList(db.SharingScopes, "SharingScopeId", "Name", assignment.SharingScopeId);
            return View(assignment);
        }

        //
        // GET: /Assignment/Edit/5

        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.LtiVersionId = new SelectList(db.LtiVersions, "LtiVersionId", "Name", assignment.LtiVersionId);
            ViewBag.SharingScopeId = new SelectList(db.SharingScopes, "SharingScopeId", "Name", assignment.SharingScopeId);
            return View(assignment);
        }

        //
        // POST: /Assignment/Edit/5

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateInput(false)]
        public ActionResult Edit(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.LtiVersionId = new SelectList(db.LtiVersions, "LtiVersionId", "Name", assignment.LtiVersionId);
            ViewBag.SharingScopeId = new SelectList(db.SharingScopes, "SharingScopeId", "Name", assignment.SharingScopeId);
            return View(assignment);
        }

        //
        // GET: /Assignment/Delete/5

        [Authorize(Roles = "Teacher")]
        public ActionResult Delete(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        //
        // POST: /Assignment/Delete/5

        [Authorize(Roles = "Teacher")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            foreach (var score in db.Scores.Where(s => s.AssignmentId == id))
                db.Scores.Remove(score);
            Assignment assignment = db.Assignments.Find(id);
            db.Assignments.Remove(assignment);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Assignment/Launch/5

        public ActionResult Launch(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
        
            // Public assignments are displayed on the front page even if the
            // user is not logged in. If the user is anonymous or if the 
            // assignment does not have an LTI key and secret defined, then
            // the Launch reverts to a simple redirect (GET). I'm curious to
            // see how providers handle this.

            if (Request.IsAuthenticated && assignment.IsLtiLink)
            {
                using (var lti = new LtiUtility())
                {
                    var ltiRequest = lti.CreateLtiRequest(assignment);
                    return View(ltiRequest);
                }
            }

            // If the user is not logged in or the assignment does not have an LTI
            // key and secret, then treat the launch as a simple link.
            return Redirect(assignment.Url);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}