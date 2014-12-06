using System.Linq;
using System.Web;
using System.Web.Mvc;
using Consumer.Models;
using Microsoft.AspNet.Identity.Owin;

namespace Consumer.Controllers
{
    public class HomeController : Controller
    {
        public HomeController() { }

        public HomeController(ConsumerContext consumerContext)
        {
            ConsumerContext = consumerContext;
        }

        private ConsumerContext _consumerContext;
        public ConsumerContext ConsumerContext
        {
            get
            {
                return _consumerContext ?? HttpContext.GetOwinContext().Get<ConsumerContext>();
            }
            private set
            {
                _consumerContext = value;
            }
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View(ConsumerContext.Courses.ToList().OrderBy(c => c.Name));
        }
    }
}