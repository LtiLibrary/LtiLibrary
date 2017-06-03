using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Membership;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Tests.Membership
{
    /// <summary>
    /// This controller does not do anything, but it does have a different
    /// controller name than MembershipController. Note that if there are two running
    /// controllers that inherit from the same base controller, at least one of 
    /// them must override its route.
    /// </summary>
    [Route("api/[controller]", Name = "MembershipsApi")]
    public class MembershipsController : MembershipControllerBase
    {
        protected override Func<GetMembershipRequest, Task<GetMembershipResponse>> OnGetMembershipAsync => null;
    }
}
