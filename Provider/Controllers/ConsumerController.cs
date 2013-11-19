using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using LtiLibrary.Models;
using Provider.Models;

namespace Provider.Controllers
{
    [Authorize]
    public class ConsumerController : Controller
    {
        private readonly ProviderContext _db = new ProviderContext();
        private const string UniqueKeyErrorMessage = "The Key field must be unique";

        //
        // GET: /Consumer/

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(_db.Consumers.ToList());
        }

        //
        // GET: /Consumer/Details/5

        public ActionResult Details(int id = 0)
        {
            Consumer consumer = _db.Consumers.Find(id);
            if (consumer == null)
            {
                return HttpNotFound();
            }
            return View(consumer);
        }

        //
        // GET: /Consumer/Create

        public ActionResult Create()
        {
            Consumer consumer = new Consumer();
            consumer.Key = Guid.NewGuid().ToString("N").Substring(0, 16);
            consumer.Secret = Guid.NewGuid().ToString("N").Substring(0, 16);
            return View(consumer);
        }

        //
        // POST: /Consumer/Create

        [HttpPost]
        public ActionResult Create(Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                // Make sure the user did not create a non-unique key
                var match = _db.Consumers.SingleOrDefault(
                    c => c.Key == consumer.Key);
                if (match != null)
                {
                    ModelState.AddModelError("Key", UniqueKeyErrorMessage);
                }
                else
                {
                    _db.Consumers.Add(consumer);
                    _db.SaveChanges();
                    if (string.IsNullOrEmpty(Request["ReturnURL"]))
                    {
                        return RedirectToAction("Index");
                    }
                    var uri = new UriBuilder(Request["ReturnURL"]);
                    uri.Query += "ConsumerId=" + consumer.ConsumerId;
                    return Redirect(uri.ToString());
                }
            }
            return View(consumer);
        }

        //
        // GET: /Consumer/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Consumer consumer = _db.Consumers.Find(id);
            if (consumer == null)
            {
                return HttpNotFound();
            }
            return View(consumer);
        }

        //
        // POST: /Consumer/Edit/5

        [HttpPost]
        public ActionResult Edit(Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                // Make sure the user did not change the Key to
                // a non-unique value
                var match = _db.Consumers.SingleOrDefault(
                    c => c.Key == consumer.Key && c.ConsumerId != consumer.ConsumerId);
                if (match != null)
                {
                    ModelState.AddModelError("Key", UniqueKeyErrorMessage);
                }
                else
                {
                    _db.Entry(consumer).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(consumer);
        }

        //
        // GET: /Consumer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Consumer consumer = _db.Consumers.Find(id);
            if (consumer == null)
            {
                return HttpNotFound();
            }
            return View(consumer);
        }

        //
        // POST: /Consumer/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Consumer consumer = _db.Consumers.Find(id);
            _db.Consumers.Remove(consumer);
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