using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Membership;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.Lis.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Tests.Membership
{
    public class MembershipController : MembershipControllerBase
    {
        protected override Func<GetMembershipDto, Task> OnGetMembership => GetMembership;

        private async Task GetMembership(GetMembershipDto dto)
        {
            // This test controller implements a very simple authorization scheme
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

            // If the Role filter is specified, only return the corresponding page
            if (dto.Role == Role.Learner)
            {
                dto.MembershipContainerPage = GetMembershipPageOfLearners();
            }
            else
            {
                var page = Request.Query["page"];
                
                // If this is the first page, return the list of instructors
                // and set the next page URL to include page=2
                if (page.Count == 0)
                {
                    dto.MembershipContainerPage = GetMembershipPageOfInstructors();
                    dto.MembershipContainerPage.NextPage = new Uri(Request.GetUri(), "/ims/membership?page=2").AbsoluteUri;
                }

                // If this is the second page, return the list of learners
                // but do not set the next page URL
                else if (page[0].Equals("2"))
                {
                    dto.MembershipContainerPage = GetMembershipPageOfLearners();
                }

                // Otherwise, I don't know what page they want
                else
                {
                    dto.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
            }
            dto.StatusCode = StatusCodes.Status200OK;
        }

        private MembershipContainerPage GetMembershipPageOfInstructors()
        {
            return new MembershipContainerPage
            {
                Id = new Uri(Request.GetUri(), $"/ims/membership/1422554502-i"),
                Differences = new Uri(Request.GetUri(), "/ims/membership?x=1422554502").AbsoluteUri,
                MembershipContainer = new MembershipContainer
                {
                    MembershipSubject = new Context
                    {
                        ContextId = "2923-abc",
                        Membership = new[]
                        {
                            new NetCore.Lis.v2.Membership
                            {
                                Member = new Person
                                {
                                    SourcedId = "school.edu:user",
                                    UserId = $"0ae836b9-7fc9-4060-006f-27b2066ac545-i",
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

        private MembershipContainerPage GetMembershipPageOfLearners()
        {
            return new MembershipContainerPage
            {
                Id = new Uri(Request.GetUri(), $"/ims/membership/1422554502-l"),
                MembershipContainer = new MembershipContainer
                {
                    MembershipSubject = new Context
                    {
                        ContextId = "2923-abc",
                        Membership = new[]
                        {
                            new NetCore.Lis.v2.Membership
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
