using System;
using System.Collections.Generic;
using System.Text;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Lis.v2;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lis.v2;
using LtiLibrary.NetCore.Lti1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LtiLibrary.AspNetCore.Tests.Lis.v2
{
    public class MembershipController : MembershipControllerBase
    {
        public MembershipController()
        {
            OnGetMemberships = async dto =>
            {
                if (!Request.IsAuthenticatedWithLti())
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var ltiRequest = await Request.ParseLtiRequestAsync();
                var signature = ltiRequest.GenerateSignature("secret");
                if (!ltiRequest.Signature.Equals(signature))
                {
                    dto.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                dto.MembershipContainerPage = new MembershipContainerPage
                {
                    Id = new Uri(Request.GetUri(), "/ims/membership/1422554502"),
                    NextPage = dto.Page.HasValue ? null : new Uri(Request.GetUri(), "/ims/membership?page=2").AbsoluteUri,
                    Differences = new Uri(Request.GetUri(), "/ims/membership?x=1422554502").AbsoluteUri,
                    MembershipContainer = new MembershipContainer
                    {
                        MembershipSubject = new Context
                        {
                            ContextId = "2923-abc",
                            Membership = new[]
                            {
                                new Membership
                                {
                                    Member = new Person
                                    {
                                        SourcedId = "school.edu:user",
                                        UserId = "0ae836b9-7fc9-4060-006f-27b2066ac545",
                                        Email = "user@school.edu",
                                        FamilyName = "Public",
                                        Name = "Jane Q. Public",
                                        Image = new Uri("http://imsglobal.org/favicon.ico"),
                                        GivenName = "Jane"
                                    },
                                    Message = new []
                                    {
                                        new
                                        {
                                            message_type = "basic-lti-launch-request",
                                            lis_result_sourcedid = "83873872987329873264783687634",
                                            ext = new
                                            {
                                                user_username = "jpublic"
                                            },
                                            custom = new
                                            {
                                                country = "Canada",
                                                user_mobile = "123-456-7890"
                                            }
                                        }
                                    },
                                    Role = new []
                                    {
                                        Role.Instructor
                                    }
                                }
                            }
                        }
                    }
                };
                dto.StatusCode = StatusCodes.Status200OK;
            };
        }
    }
}
