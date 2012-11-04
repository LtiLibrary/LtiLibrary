using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Consumer.Models;

namespace Consumer.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ConsumerContext db = new ConsumerContext();

        //
        // GET: /User/

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var user = db.Users.Find(id);
            user.IsStudent = Roles.IsUserInRole(UserRoles.StudentRole);
            user.IsTeacher = Roles.IsUserInRole(UserRoles.TeacherRole);

            // Always pre-load the states
            ViewBag.StateId = new SelectList(db.States.ToList(), "StateId", "Name", user.StateId);
            ViewBag.DistrictId = new SelectList(new SelectListItem[] {});
            ViewBag.SchoolId = new SelectList(new SelectListItem[] { });
            // Only load the districts if the state has already been selected
            if (!string.IsNullOrEmpty(user.StateId))
            {
                ViewBag.DistrictId = new SelectList(
                    db.Districts.Where(i => i.StateId.Equals(user.StateId)).OrderBy(i => i.Name),
                    "DistrictId", "Name", user.DistrictId
                    );
            }
            // Only load the schools if the district has already been selected
            if (!string.IsNullOrEmpty(user.DistrictId))
            {
                ViewBag.SchoolId = new SelectList(
                    db.Schools.Where(i => i.DistrictId.Equals(user.DistrictId)).OrderBy(i => i.Name),
                    "SchoolId", "Name", user.SchoolId
                    );
            }

            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                if (user.IsStudent)
                {
                    if (!Roles.IsUserInRole(user.UserName, UserRoles.StudentRole))
                        Roles.AddUserToRole(user.UserName, UserRoles.StudentRole);
                }
                else
                {
                    if (Roles.IsUserInRole(user.UserName, UserRoles.StudentRole))
                        Roles.RemoveUserFromRole(user.UserName, UserRoles.StudentRole);
                }

                if (user.IsTeacher)
                {
                    if (!Roles.IsUserInRole(user.UserName, UserRoles.TeacherRole))
                        Roles.AddUserToRole(user.UserName, UserRoles.TeacherRole);
                }
                else
                {
                    if (Roles.IsUserInRole(user.UserName, UserRoles.TeacherRole))
                        Roles.RemoveUserFromRole(user.UserName, UserRoles.TeacherRole);
                }
                
                return RedirectToAction("Index", "Home");
            }
            user.IsStudent = Roles.IsUserInRole(user.UserName, UserRoles.StudentRole);
            user.IsTeacher = Roles.IsUserInRole(user.UserName, UserRoles.TeacherRole);

            // Always pre-load the states
            ViewBag.StateId = new SelectList(db.States.ToList(), "StateId", "Name", user.StateId);
            ViewBag.DistrictId = new SelectList(new SelectListItem[] { });
            ViewBag.SchoolId = new SelectList(new SelectListItem[] { });
            // Only load the districts if the state has already been selected
            if (!string.IsNullOrEmpty(user.StateId))
            {
                ViewBag.DistrictId = new SelectList(
                    db.Districts.Where(i => i.StateId.Equals(user.StateId)).OrderBy(i => i.Name),
                    "DistrictId", "Name", user.DistrictId
                    );
            }
            // Only load the schools if the district has already been selected
            if (!string.IsNullOrEmpty(user.DistrictId))
            {
                ViewBag.SchoolId = new SelectList(
                    db.Schools.Where(i => i.DistrictId.Equals(user.DistrictId)).OrderBy(i => i.Name),
                    "SchoolId", "Name", user.SchoolId
                    );
            }
            return View(user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}