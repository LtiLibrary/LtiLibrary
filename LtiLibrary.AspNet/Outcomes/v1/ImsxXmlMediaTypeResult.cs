using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNet.Outcomes.v1
{
    public class ImsxXmlMediaTypeResult : ObjectResult
    {
        public ImsxXmlMediaTypeResult(object value) : base(value)
        {
            Formatters.Add(new ImsxXmlMediaTypeOutputFormatter());
        }
    }
}
