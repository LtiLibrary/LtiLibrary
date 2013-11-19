using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Consumer.Models;
using Consumer.Utility;
using LtiLibrary.Common;
using LtiLibrary.Consumer;
using WebMatrix.WebData;

namespace Consumer.Lti
{
    public static class LtiUtility
    {
        public static LtiOutboundRequestViewModel CreateLtiRequest(Assignment assignment)
        {
            using (var db = new ConsumerContext())
            {
                var ltiRequest = new LtiOutboundRequest
                {
                    ConsumerKey = assignment.ConsumerKey,
                    ConsumerSecret = assignment.ConsumerSecret,
                    ResourceLinkId = assignment.AssignmentId.ToString(CultureInfo.InvariantCulture),
                    Url = assignment.Url
                };

                var course = assignment.Course;
                var user = db.Users.Find(WebSecurity.CurrentUserId);

                // Tool
                ltiRequest.ToolConsumerInfoProductFamilyCode = "consumer.azurewebsites.net";
                ltiRequest.ToolConsumerInfoVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                // Context
                ltiRequest.ContextId = course.CourseId.ToString(CultureInfo.InvariantCulture);
                ltiRequest.ContextTitle = course.Name;
                ltiRequest.ContextType = course.EnrolledUsers.Any(u => u.UserId == user.UserId) 
                    ? LisContextTypes.CourseSection : LisContextTypes.CourseTemplate;

                // Instance
                ltiRequest.ToolConsumerInstanceGuid = "~/".ToAbsoluteUrl();
                var titles = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyTitleAttribute>();
                var assemblyTitleAttributes = titles as AssemblyTitleAttribute[] ?? titles.ToArray();
                if (assemblyTitleAttributes.Any())
                {
                    ltiRequest.ToolConsumerInstanceName = assemblyTitleAttributes.First().Title;
                }
                ltiRequest.ResourceLinkTitle = assignment.Name;
                ltiRequest.ResourceLinkDescription = assignment.Description;

                // User
                if (user.SendEmail.GetValueOrDefault(true))
                {
                    ltiRequest.LisPersonEmailPrimary = user.Email;
                }
                if (user.SendName.GetValueOrDefault(true))
                {
                    ltiRequest.LisPersonNameFamily = user.LastName;
                    ltiRequest.LisPersonNameGiven = user.FirstName;
                }
                ltiRequest.UserId = user.UserId.ToString(CultureInfo.InvariantCulture);
                ltiRequest.AddRoles(GetLtiRolesForUser());

                // Outcomes service
                var request = HttpContext.Current.Request;
                var urlHelper = new UrlHelper(request.RequestContext);
                if (course.EnrolledUsers.Any(u => u.UserId == user.UserId))
                {
                    var outcomesUrl = urlHelper.HttpRouteUrl("DefaultApi", new {controller = "Outcomes"});
                    if (!string.IsNullOrEmpty(outcomesUrl))
                    {
                        ltiRequest.LisOutcomeServiceUrl =
                            new Uri(request.Url, outcomesUrl).AbsoluteUri;
                        ltiRequest.LisResultSourcedId = string.Format("{0}-{1}", assignment.AssignmentId, user.UserId);
                    }
                }

                // Tool Consumer Profile service
                var profileUrl = urlHelper.HttpRouteUrl("DefaultApi", new {controller = "ToolConsumerProfile"});
                if (!string.IsNullOrEmpty(profileUrl))
                {
                    ltiRequest.ToolConsumerProfileUrl = string.Format("{0}?lti_version=LTI-1p2", new Uri(request.Url, profileUrl).AbsoluteUri);
                }

                // Add the custom parameters. This consumer supports 3 special substitutions.
                var customParameters = assignment.CustomParameters;

                if (!string.IsNullOrWhiteSpace(customParameters))
                {
                    // Friendly version of the username
                    customParameters = Regex.Replace(customParameters, "\\$User.username",
                                                     user.UserName ?? string.Empty, RegexOptions.IgnoreCase);

                    // The source of the following data is the NCES Elementary and 
                    // Secondary School Information System (nces.ed.gov/ccd/elsi/).
                    var leaNcesId = course.District == null ? null : course.District.DistrictId;
                    var schoolNcesId = course.School == null ? null : course.School.SchoolId;
                    var stateId = course.State == null ? null : course.State.StateId;

                    customParameters = Regex.Replace(customParameters, "\\$Context.stateId",
                                                     stateId ?? string.Empty, RegexOptions.IgnoreCase);
                    customParameters = Regex.Replace(customParameters, "\\$Context.ncesLeaId",
                                                     string.IsNullOrWhiteSpace(leaNcesId)
                                                         ? string.Empty
                                                         : "nces.ed.gov:" + leaNcesId,
                                                     RegexOptions.IgnoreCase);
                    customParameters = Regex.Replace(customParameters, "\\$Context.ncesSchoolId",
                                                     string.IsNullOrWhiteSpace(schoolNcesId)
                                                         ? string.Empty
                                                         : "nces.ed.gov:" + schoolNcesId,
                                                     RegexOptions.IgnoreCase);

                    ltiRequest.AddCustomParameters(customParameters);
                }

                return ltiRequest.GetLtiRequestModel();
            }
        }

        private static IList<LtiRoles> GetLtiRolesForUser()
        {
            var roles = new List<LtiRoles>();
            if (Roles.IsUserInRole(UserRoles.StudentRole)) roles.Add(LtiRoles.Learner);
            if (Roles.IsUserInRole(UserRoles.TeacherRole)) roles.Add(LtiRoles.Instructor);
            return roles;
        }
    }
}