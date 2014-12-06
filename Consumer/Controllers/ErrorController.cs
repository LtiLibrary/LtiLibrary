using System.Web;
using System.Web.Mvc;

namespace Consumer.Controllers
{
    public class ErrorController : Controller
    {
        [ValidateInput(false)]
        public ActionResult BadRequest(string error)
        {
            var controllerName = (string)RouteData.Values["controller"];
            var actionName = (string)RouteData.Values["action"];
            var model = new HandleErrorInfo(new HttpException(500, error), controllerName, actionName);
            return View(model);
        }
	}
}