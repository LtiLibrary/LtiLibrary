using System.Web.Mvc;
using System.Web.Routing;

namespace Provider
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Create",
            //    url: "Tool/Create",
            //    defaults: new { controller = "Tool", action = "Create" }
            //);

            // This route creates a short URL to a specific tool.
            // this makes it easy to copy and paste.
            // For example, http://localhost/Tool/5
            routes.MapRoute(
                name: "Launch",
                url: "Tool/{id}", 
                defaults: new { controller = "Tool", action = "View" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}