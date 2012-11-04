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
            if (Request.IsAuthenticated)
            {
                var currentUser = db.Users.Find(WebSecurity.CurrentUserId);
                return View(db.Assignments.Where(
                    a => a.UserId == WebSecurity.CurrentUserId
                        || (a.SharingScopeId == SharingScope.District && a.User.DistrictId == currentUser.DistrictId)
                        || (a.SharingScopeId == SharingScope.School && a.User.SchoolId == currentUser.SchoolId)
                        || (a.SharingScopeId == SharingScope.State && a.User.StateId == currentUser.StateId)
                        || (a.SharingScopeId == SharingScope.Public)
                    ).OrderBy(a => a.Name).ToList());
            }
            return View(db.Assignments.Where(a => a.SharingScopeId == SharingScope.Public).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}