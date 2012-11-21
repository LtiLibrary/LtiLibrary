using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Consumer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Use XmlSerializer instead of DataContractSerializer
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
#if DEBUG
            config.Formatters.XmlFormatter.Indent = true;
#endif

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
