using System;
using System.Web.Http.Controllers;

namespace LtiLibrary.AspNet.Outcomes.v2
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResultsControllerConfigAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            // Add the ResultFormatter to the associated ApiController. This formatter
            // looks for the LisResultMediaType in the request Accept header and responds
            // with the corresponding Content-Type.
            controllerSettings.Formatters.Add(new ResultFormatter());
        }
    }
}
