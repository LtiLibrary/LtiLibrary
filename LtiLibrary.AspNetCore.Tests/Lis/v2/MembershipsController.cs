using System;
using System.Collections.Generic;
using System.Text;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Lis.v2;
using LtiLibrary.NetCore.Lis.v2;
using LtiLibrary.NetCore.Lti1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Lis.v2
{
    public class MembershipsController : MembershipsControllerBase
    {
        public MembershipsController()
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
                    Id = new Uri("http://lms.example.com/sections/2923/memberships/?rlid=49566-rkk96"),
                    NextPage = "http://lms.example.com/sections/2923/memberships/?rlid=49566-rkk96&p=2",
                    Differences = "http://lms.example.com/sections/2923/memberships/?x=1422554502",
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
