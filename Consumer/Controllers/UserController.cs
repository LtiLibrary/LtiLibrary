using Consumer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Consumer.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public UserController() { }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /User/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(UserManager.Users.ToList());
        }

        //
        // GET: /User/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = User.Identity.GetUserId();
            }
            var user = UserManager.FindById(id);
            var model = new UserProfileModel(user);
            model.IsStudent = UserManager.IsInRole(user.Id, UserRoles.StudentRole);
            model.IsTeacher = UserManager.IsInRole(user.Id, UserRoles.TeacherRole);
            return View(model);
        }

        //
        // POST: /User/Edit/5
        [HttpPost]
        public ActionResult Edit(UserProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(model.UserId);
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.SendEmail = model.SendEmail;
                user.SendName = model.SendName;

                UserManager.Update(user);

                UpdateUserRole(user.Id, UserRoles.StudentRole, model.IsStudent);
                UpdateUserRole(user.Id, UserRoles.TeacherRole, model.IsTeacher);

                if (User.Identity.GetUserId() == user.Id)
                {
                    var claimsIdentity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    Request.GetOwinContext().Authentication.SignIn(claimsIdentity);
                }

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        private void UpdateUserRole(string userId, string roleName, bool userIsInRole)
        {
            Debug.WriteLine("{0} is in role {1}: {2}", userId, roleName, UserManager.IsInRole(userId, roleName));
            if (userIsInRole)
            {
                if (!UserManager.IsInRole(userId, roleName))
                    UserManager.AddToRole(userId, roleName);
            }
            else
            {
                if (UserManager.IsInRole(userId, roleName))
                    UserManager.RemoveFromRole(userId, roleName);
            }
        }

        //
        // GET: /User/Delete/5
        [Authorize(Roles = "SuperUser")]
        public ActionResult Delete(string id)
        {
            var user = UserManager.FindById(id);
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
        public ActionResult DeleteConfirmed(string id)
        {
            var user = UserManager.FindById(id);

            // Remove this user from any roles they may have
            foreach (var role in Roles.GetRolesForUser(user.UserName))
            {
                Roles.RemoveUserFromRole(user.UserName, role);
            }

            // Remove the user's account
            Membership.DeleteUser(user.UserName, true);

            return RedirectToAction("Index", "Home");
        }
    }
}