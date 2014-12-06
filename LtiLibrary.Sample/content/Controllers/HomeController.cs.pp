using System.Web.Mvc;

namespace $rootnamespace$.Controllers
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