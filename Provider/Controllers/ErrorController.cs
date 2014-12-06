using System.Web;
using System.Web.Mvc;
using Provider.Filters;

namespace Provider.Controllers
{
    [XFrameOptionsHeader]
    public class ErrorController : Controller
    {
        public ActionResult BadRequest(string error)
        {
            var controllerName = (string)RouteData.Values["controller"];
            var actionName = (string)RouteData.Values["action"];
            var model = new HandleErrorInfo(new HttpException(500, error), controllerName, actionName);
            return View(model);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            var name = GetViewName(ControllerContext, string.Format("~/Views/Error/{0}.cshtml", actionName),
                                            "~/Views/Error/Error.cshtml",
                                            "~/Views/Error/General.cshtml",
                                            "~/Views/Shared/Error.cshtml");

            var controllerName = (string)RouteData.Values["controller"];
            var model = new HandleErrorInfo(Server.GetLastError(), controllerName, actionName);
            var result = new ViewResult
            {
                ViewName = name,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            };


            Response.StatusCode = 501;
            result.ExecuteResult(ControllerContext);
        }

        protected string GetViewName(ControllerContext context, params string[] names)
        {
            foreach (var name in names)
            {
                var result = ViewEngines.Engines.FindView(ControllerContext, name, null);
                if (result.View != null)
                    return name;
            }
            return null;
        }
    }
}
