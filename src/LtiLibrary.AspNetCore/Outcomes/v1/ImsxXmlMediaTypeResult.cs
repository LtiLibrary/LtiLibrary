using LtiLibrary.NetCore.Lti.v1;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v1
{
    /// <summary>
    /// Use to attach a specific XML formatter to controller results. For example,
    /// <code>public ImsxXmlMediaTypeResult Post([ModelBinder(BinderType = typeof(ImsxXmlMediaTypeModelBinder))] imsx_POXEnvelopeType request)</code>
    /// </summary>
    internal class ImsxXmlMediaTypeResult : ObjectResult
    {
        public ImsxXmlMediaTypeResult(imsx_POXEnvelopeType value) : base(value)
        {
            // This formatter produces an imsx_POXEnvelopeType with an imsx_POXEnvelopeResponse
            Formatters.Add(new ImsxXmlMediaTypeOutputFormatter());
        }

        public imsx_POXEnvelopeType Response
        {
            get
            {
                return Value as imsx_POXEnvelopeType;
            }
        }
    }
}
