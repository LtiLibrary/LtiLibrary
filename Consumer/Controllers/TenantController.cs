using System.Data;
using System.Linq;
using System.Web.Mvc;
using inBloomLibrary.Models;
using WebMatrix.WebData;

namespace Consumer.Controllers
{
    [Authorize]
    public class TenantController : Controller
    {
        private readonly inBloomLibraryContext _db = new inBloomLibraryContext();

        //
        // GET: /Tenant/

        public ActionResult Index()
        {
            return View(_db.Tenants.Where(t => t.UserId == WebSecurity.CurrentUserId).ToList());
        }

        //
        // GET: /Tenant/Details/junk@junk.com

        public ActionResult Details(int id)
        {
            Tenant tenant = _db.Tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            if (tenant.UserId != WebSecurity.CurrentUserId)
            {
                return new HttpUnauthorizedResult();
            }
            return View(tenant);
        }

        //
        // GET: /Tenant/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Tenant/Create

        [HttpPost]
        public ActionResult Create(Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                if (_db.Tenants.Count(t => t.inBloomTenantId == tenant.inBloomTenantId) == 0)
                {
                    tenant.UserId = WebSecurity.CurrentUserId;
                    _db.Tenants.Add(tenant);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("SlcTenantId", "This tenant is already registered.");
            }

            return View(tenant);
        }

        //
        // GET: /Tenant/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Tenant tenant = _db.Tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            if (tenant.UserId != WebSecurity.CurrentUserId)
            {
                return new HttpUnauthorizedResult();
            }
            return View(tenant);
        }

        //
        // POST: /Tenant/Edit/5

        [HttpPost]
        public ActionResult Edit(Tenant tenant)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(tenant).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tenant);
        }

        //
        // GET: /Tenant/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Tenant tenant = _db.Tenants.Find(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            if (tenant.UserId != WebSecurity.CurrentUserId)
            {
                return new HttpUnauthorizedResult();
            }
            return View(tenant);
        }

        //
        // POST: /Tenant/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Tenant tenant = _db.Tenants.Find(id);
            _db.Tenants.Remove(tenant);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}