using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Consumer.Models;
using Microsoft.Web.WebPages.OAuth;

namespace Consumer.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ConsumerContext _db = new ConsumerContext();

        //
        // GET: /User/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(_db.Users.ToList());
        }

        //
        // GET: /User/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id = 0)
        {
            User user = _db.Users.Find(id);
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
            var user = _db.Users.Find(id);
            var model = new UserProfileModel(user);
            model.IsStudent = Roles.IsUserInRole(user.UserName, UserRoles.StudentRole);
            model.IsTeacher = Roles.IsUserInRole(user.UserName, UserRoles.TeacherRole);
            return View(model);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(UserProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.Find(model.UserId);
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.SendEmail = model.SendEmail;
                user.SendName = model.SendName;

                _db.Entry(user).State = EntityState.Modified;
                _db.SaveChanges();

                UpdateUserRole(user.UserName, UserRoles.StudentRole, model.IsStudent);
                UpdateUserRole(user.UserName, UserRoles.TeacherRole, model.IsTeacher);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        private void UpdateUserRole(string username, string roleName, bool userIsInRole)
        {
            if (userIsInRole)
            {
                if (!Roles.IsUserInRole(username, roleName))
                    Roles.AddUserToRole(username, roleName);
            }
            else
            {
                if (Roles.IsUserInRole(username, roleName))
                    Roles.RemoveUserFromRole(username, roleName);
            }
        }

        //
        // GET: /User/Delete/5
        [Authorize(Roles = "SuperUser")]
        public ActionResult Delete(int id = 0)
        {
            User user = _db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5
        [Authorize(Roles = "SuperUser")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = _db.Users.Find(id);

            // Remove OAuth credentials if they exist
            foreach (var account in OAuthWebSecurity.GetAccountsFromUserName(user.UserName))
            {
                OAuthWebSecurity.DeleteAccount(account.Provider, account.ProviderUserId);
            }

            // Remove this user from any roles they may have
            foreach (var role in Roles.GetRolesForUser(user.UserName))
            {
                Roles.RemoveUserFromRole(user.UserName, role);
            }

            // Remove the user's account
            Membership.DeleteUser(user.UserName, true);

            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}