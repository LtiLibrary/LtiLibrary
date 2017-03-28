using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNet.Outcomes.v1
{
    public class ImsxXmlResult : ObjectResult
    {
        public ImsxXmlResult(object value) : base(value)
        {
            Formatters.Add(new ImsxXmlMediaTypeFormatter());
        }
    }
}
