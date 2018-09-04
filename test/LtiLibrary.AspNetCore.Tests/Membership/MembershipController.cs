using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.AspNetCore.Membership;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.Lis.v2;

namespace LtiLibrary.AspNetCore.Tests.Membership
{
    public class MembershipController : MembershipControllerBase
    {
        protected override Func<GetMembershipRequest, Task<GetMembershipResponse>> OnGetMembershipAsync => GetMembershipAsync;

        private async Task<GetMembershipResponse> GetMembershipAsync(GetMembershipRequest request)
        {
            var response = new GetMembershipResponse();

            // This test controller implements a very simple authorization scheme
            var ltiRequest = await Request.ParseLtiRequestAsync();
            var signature = ltiRequest.GenerateSignature("secret");
            if (!ltiRequest.Signature.Equals(signature))
            {
                return Unauthorized();
            }

            // If the contextId is unknown, return NotFound
            if (!request.ContextId.Equals("context-1"))
            {
                return NotFound($"Cannot find {nameof(request.ContextId)}");
            }

            switch (request.Role)
            {
                // If the Role filter is specified, only return the corresponding page
                case ContextRole.Learner:
                    response.MembershipContainerPage = GetMembershipPageOfLearners();
                    break;
                case ContextRole.Instructor:
                    response.MembershipContainerPage = GetMembershipPageOfInstructors();
                    break;
                default:
                    var page = Request.Query["page"];
                
                    // If this is the first page, return the list of instructors
                    // and set the next page URL to include page=2
                    if (page.Count == 0)
                    {
                        response.MembershipContainerPage = GetMembershipPageOfInstructors();
                        response.MembershipContainerPage.NextPage = new Uri(Request.GetUri(), "/ims/membership/context/context-1?page=2").AbsoluteUri;
                    }

                    // If this is the second page, return the list of learners
                    // but do not set the next page URL
                    else if (page[0].Equals("2"))
                    {
                        response.MembershipContainerPage = GetMembershipPageOfLearners();
                    }

                    // Otherwise, we don't know what page they want
                    else
                    {
                        return NotFound($"Cannot find page {page}");
                    }

                    break;
            }
            return response;
        }

        private MembershipContainerPage GetMembershipPageOfInstructors()
        {
            return new MembershipContainerPage
            {
                Id = new Uri(Request.GetUri(), "/ims/membership/context/context-1?page=1"),
                Differences = new Uri(Request.GetUri(), "/ims/membership/context/context-1?diff=1422554502").AbsoluteUri,
                MembershipContainer = new MembershipContainer
                {
                    MembershipSubject = new Context
                    {
                        ContextId = "context-1",
                        Membership = new[]
                        {
                            new NetCore.Lis.v2.Membership
                            {
                                Member = new Person
                                {
                                    SourcedId = "sis:942a8dd9",
                                    UserId = "instructor-1",
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
                                    ContextRole.Instructor
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
                Id = new Uri(Request.GetUri(), "/ims/membership/context/context-1?page-2"),
                MembershipContainer = new MembershipContainer
                {
                    MembershipSubject = new Context
                    {
                        ContextId = "context-1",
                        Membership = new[]
                        {
                            new NetCore.Lis.v2.Membership
                            {
                                Member = new Person
                                {
                                    SourcedId = "sis:123abc456",
                                    UserId = "student-1",
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
                                    ContextRole.Learner
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
