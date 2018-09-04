using System.Collections.Generic;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Lis.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Membership
{
    /// <summary>
    /// Represents a GetMembership result.
    /// </summary>
    public class GetMembershipResponse
    {
        /// <summary>
        /// Create an empty response
        /// </summary>
        public GetMembershipResponse()
        {
            StatusCode = StatusCodes.Status200OK;
        }

        /// <summary>
        /// Create a fully formed response
        /// </summary>
        public GetMembershipResponse(HttpRequest request, string contextId) : this()
        {
            MembershipContainerPage = new MembershipContainerPage
            {
                Id = request.GetUri(),
                MembershipContainer = new MembershipContainer
                {
                    MembershipSubject = new Context
                    {
                        ContextId = $"{contextId}",
                        Membership = new List<NetCore.Lis.v2.Membership>()
                    }
                }
            };
        }

        /// <summary>
        /// Get or set the MembershipContainerPage.
        /// </summary>
        public MembershipContainerPage MembershipContainerPage { get; set; }

        /// <summary>
        /// Get membership from within MembershipContainerPage.
        /// </summary>
        public ICollection<NetCore.Lis.v2.Membership> Membership
        {
            get
            {
                return MembershipContainerPage
                    ?.MembershipContainer
                    ?.MembershipSubject
                    ?.Membership;
            }
        }

        /// <summary>
        /// Get or set the HTTP status code representing the success or failure of the getting the membership.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The value to 
        /// </summary>
        public object StatusValue { get; set; }
    }
}
