using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// Use to attach a specific XML formatter to controller results. For example,
    /// <code>public ImsxXmlMediaTypeResult Post([ModelBinder(BinderType = typeof(ImsxXmlMediaTypeModelBinder))] imsx_POXEnvelopeType request)</code>
    /// </summary>
    public class ImsxXmlMediaTypeResult : ObjectResult
    {
        public ImsxXmlMediaTypeResult(object value) : base(value)
        {
            // This formatter produces an imsx_POXEnvelopeType with an imsx_POXEnvelopeResponse
            Formatters.Add(new ImsxXmlMediaTypeOutputFormatter());
        }
    }
}
