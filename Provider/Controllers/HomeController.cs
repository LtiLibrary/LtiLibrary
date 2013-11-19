using System.Linq;
using System.Web.Mvc;
using Provider.Models;

namespace Provider.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProviderContext _db = new ProviderContext();

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(_db.Tools.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
