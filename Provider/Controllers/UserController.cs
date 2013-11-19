using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Provider.Models;
using Microsoft.Web.WebPages.OAuth;

namespace Provider.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ProviderContext _db = new ProviderContext();

        //
        // GET: /User/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(_db.Users.ToList());
        }

        //
        // GET: /User/Details/5

        [Authorize(Roles = "SuperUser")]
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
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(User model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.Find(model.UserId);
                user.Email = model.Email;
                user.FirstName = model.Email;
                user.LastName = model.LastName;

                _db.Entry(user).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View(model);
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

            // Remove any pairing records
            foreach (var pairedUser in _db.PairedUsers.Where(u => u.User == user).ToList())
            {
                _db.PairedUsers.Remove(pairedUser);
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