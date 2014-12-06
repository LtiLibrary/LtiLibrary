using System.Web.Mvc;

namespace SimpleLti12.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Return the default view for the website.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}