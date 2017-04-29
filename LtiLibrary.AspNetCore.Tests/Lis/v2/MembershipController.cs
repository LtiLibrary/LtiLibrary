using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Lis.v2;
using LtiLibrary.NetCore.Lis.v2;
using LtiLibrary.NetCore.Lti1;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Lis.v2
{
    public class MembershipController : MembershipControllerBase
    {
        public MembershipController()
        {
            OnGetMembership = GetMembership;
        }

        private async Task GetMembership(GetMembershipDto dto)
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

            if (dto.Role == Role.Learner)
            {
                dto.MembershipContainerPage = GetMembershipPage2(dto);
            }
            else
            {
                dto.MembershipContainerPage = dto.Page.HasValue ? GetMembershipPage2(dto) : GetMembershipPage1(dto);
            }
            dto.StatusCode = StatusCodes.Status200OK;
        }

        private MembershipContainerPage GetMembershipPage1(GetMembershipDto dto)
        {
            return new MembershipContainerPage
            {
                Id = new Uri(Request.GetUri(), $"/ims/membership/1422554502{dto.Page}"),
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
                                    UserId = $"0ae836b9-7fc9-4060-006f-27b2066ac545{dto.Page}",
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
        }

        private MembershipContainerPage GetMembershipPage2(GetMembershipDto dto)
        {
            return new MembershipContainerPage
            {
                Id = new Uri(Request.GetUri(), $"/ims/membership/1422554502{dto.Page}"),
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
                                    UserId = "0ae836b9-7fc9-4060-006f-27b2066ac5452",
                                    Email = "user@school.edu",
                                    FamilyName = "Public",
                                    Name = "John Q. Public",
                                    Image = new Uri("http://imsglobal.org/favicon.ico"),
                                    GivenName = "John"
                                },
                                Message = new []
                                {
                                    new
                                    {
                                        message_type = "basic-lti-launch-request",
                                        lis_result_sourcedid = "838738729873298732647836876342",
                                        ext = new
                                        {
                                            user_username = "jpublic2"
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
                                    Role.Learner
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
