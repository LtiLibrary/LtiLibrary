using System.Linq;
using System.Web;
using System.Web.Mvc;
using Provider.Models;
using Microsoft.AspNet.Identity.Owin;

namespace Provider.Controllers
{
    public class HomeController : Controller
    {
        public ProviderContext ProviderContext
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ProviderContext>();
            }
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View(ProviderContext.Tools.ToList().OrderBy(t => t.Name));
        }
    }
}
