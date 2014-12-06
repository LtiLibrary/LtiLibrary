using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Provider.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Provider.Controllers
{
    [Authorize]
    public class ConsumerController : Controller
    {
        private const string UniqueKeyErrorMessage = "The Key field must be unique";

        public ConsumerController() {}

        public ConsumerController(ProviderContext providerContext)
        {
            ProviderContext = providerContext;
        }

        private ProviderContext _providerContext;
        public ProviderContext ProviderContext
        {
            get
            {
                return _providerContext ?? HttpContext.GetOwinContext().Get<ProviderContext>();
            }
            set
            {
                _providerContext = value;
            }
        }

        //
        // GET: /Consumer/

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(ProviderContext.Consumers.ToList());
        }

        //
        // GET: /Consumer/Details/5

        public ActionResult Details(int id = 0)
        {
            Consumer consumer = ProviderContext.Consumers.Find(id);
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
                var match = ProviderContext.Consumers.SingleOrDefault(
                    c => c.Key == consumer.Key);
                if (match != null)
                {
                    ModelState.AddModelError("Key", UniqueKeyErrorMessage);
                }
                else
                {
                    ProviderContext.Consumers.Add(consumer);
                    ProviderContext.SaveChanges();
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
            Consumer consumer = ProviderContext.Consumers.Find(id);
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
                var match = ProviderContext.Consumers.SingleOrDefault(
                    c => c.Key == consumer.Key && c.ConsumerId != consumer.ConsumerId);
                if (match != null)
                {
                    ModelState.AddModelError("Key", UniqueKeyErrorMessage);
                }
                else
                {
                    ProviderContext.Entry(consumer).State = EntityState.Modified;
                    ProviderContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(consumer);
        }

        //
        // GET: /Consumer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Consumer consumer = ProviderContext.Consumers.Find(id);
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
            Consumer consumer = ProviderContext.Consumers.Find(id);
            ProviderContext.Consumers.Remove(consumer);
            ProviderContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}