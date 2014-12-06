using System.Web.Mvc;

namespace Provider.Filters
{
    public class XFrameOptionsHeaderAttribute : ActionFilterAttribute
    {
        private const string XFrameOptionHeader = "x-frame-options";

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer != null)
            {
                var xFrameOption = "ALLOW FROM " + filterContext.HttpContext.Request.UrlReferrer.Authority;
                var response = filterContext.HttpContext.Response;
                if (response.Headers[XFrameOptionHeader] == null)
                {
                    filterContext.HttpContext.Response.AppendHeader(XFrameOptionHeader, "SAMEORIGIN");
                }
                response.Headers[XFrameOptionHeader] = xFrameOption;
            }
            base.OnResultExecuted(filterContext);
        }
    }
}