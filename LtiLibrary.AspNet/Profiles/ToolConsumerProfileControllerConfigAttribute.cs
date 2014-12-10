using System;
using System.Web.Http.Controllers;
using LtiLibrary.Core.Profiles;

namespace LtiLibrary.AspNet.Profiles
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ToolConsumerProfileControllerConfigAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            // Add the ToolConsumerProfileFormatter to the associated ApiController. This formatter
            // look for the ToolConsumerProfileMediaType in the request Accept header and respond
            // with the corresponding Content-Type.
            controllerSettings.Formatters.Add(new ToolConsumerProfileFormatter());
        }
    }
}
