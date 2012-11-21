using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Consumer.Models;
using WebMatrix.WebData;

namespace Consumer.Controllers
{
    public class HomeController : Controller
    {
        private ConsumerContext db = new ConsumerContext();

        //
        // GET: /Home/

        public ActionResult Index()
        {
            var scoredAssignments = new List<ScoredAssignment>();

            if (Request.IsAuthenticated)
            {
                var currentUser = db.Users.Find(WebSecurity.CurrentUserId);
                var assignments = db.Assignments.Where(
                    a => a.UserId == WebSecurity.CurrentUserId
                        || (a.SharingScopeId == SharingScope.District && a.User.DistrictId == currentUser.DistrictId)
                        || (a.SharingScopeId == SharingScope.School && a.User.SchoolId == currentUser.SchoolId)
                        || (a.SharingScopeId == SharingScope.State && a.User.StateId == currentUser.StateId)
                        || (a.SharingScopeId == SharingScope.Public)
                    ).OrderBy(a => a.Name).ToList();

                foreach (var assignment in assignments)
                {
                    var scoredAssignment = new ScoredAssignment(assignment);
                    var score = db.Scores.FirstOrDefault(
                        s => s.AssignmentId == assignment.AssignmentId && s.UserId == currentUser.UserId);
                    if (score != null)
                        scoredAssignment.Score = score.DecimalValue.ToString();
                    scoredAssignments.Add(scoredAssignment);
                }
            }
            else
            {
                var assignments = db.Assignments.Where(a => a.SharingScopeId == SharingScope.Public).ToList();
                foreach (var assignment in assignments)
                {
                    scoredAssignments.Add(new ScoredAssignment(assignment));
                }
            }
            return View(scoredAssignments);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}