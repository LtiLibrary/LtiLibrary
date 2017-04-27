using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Lis.v2
{
    /// <summary>
    /// A <see cref="Controller" /> that implements 
    /// "A REST API for LineItem Resources in multiple formats, Internal Draft 2.1"
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LineItem/service.html
    /// </summary>
    [AddBodyHashHeader]
    [Route("ims/courses/{contextId}/lineitems/{id?}", Name = "LineItemsApi")]
    [Consumes(LtiConstants.LisLineItemMediaType, LtiConstants.LisLineItemResultsMediaType, LtiConstants.LisLineItemContainerMediaType)]
    [Produces(LtiConstants.LisLineItemMediaType, LtiConstants.LisLineItemResultsMediaType, LtiConstants.LisLineItemContainerMediaType)]
    public class MembershipsControllerBase : Controller
    {
    }
}
