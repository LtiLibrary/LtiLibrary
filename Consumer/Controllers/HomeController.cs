using System.Linq;
using System.Web.Mvc;
using Consumer.Models;

namespace Consumer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConsumerContext _db = new ConsumerContext();

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(_db.Courses.ToList().OrderBy(c => c.Title));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}