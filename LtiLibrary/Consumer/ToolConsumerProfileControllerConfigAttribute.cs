using System;
using System.Web.Http.Controllers;
using Newtonsoft.Json;

namespace LtiLibrary.Consumer
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ToolConsumerProfileControllerConfigAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            var profileFormatter = new ToolConsumerProfileFormatter();
            profileFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
#if DEBUG
            profileFormatter.Indent = true;
#endif
            controllerSettings.Formatters.Clear();
            controllerSettings.Formatters.Add(profileFormatter);
        }
    }
}
