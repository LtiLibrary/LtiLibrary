using Consumer.Models;
using LtiLibrary.Common;
using LtiLibrary.Extensions;
using LtiLibrary.Lti1;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Consumer.Lti
{
    public static class LtiUtility
    {
        /// <summary>
        /// Create a basic launch request for the assignment.
        /// </summary>
        /// <param name="request">The browser request.</param>
        /// <param name="assignment">The assignment to launch.</param>
        /// <param name="user">The user that is launching the assignment.</param>
        /// <returns>An LtiRequestViewModel which can be displayed by the View in such
        /// a way that the browser will invoke the LTI launch.</returns>
        public static LtiRequestViewModel CreateBasicLaunchRequestViewModel(HttpRequestBase request, Assignment assignment, ApplicationUser user)
        {
            var ltiRequest = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                ConsumerKey = assignment.ConsumerKey,
                ResourceLinkId = assignment.AssignmentId.ToString(CultureInfo.InvariantCulture),
                Url = new Uri(assignment.Url)
            };

            var course = assignment.Course;

            // Tool
            ltiRequest.ToolConsumerInfoProductFamilyCode = Assembly.GetExecutingAssembly().GetName().Name;
            ltiRequest.ToolConsumerInfoVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Context
            ltiRequest.ContextId = course.CourseId.ToString(CultureInfo.InvariantCulture);
            ltiRequest.ContextTitle = course.Name;
            ltiRequest.ContextType = course.EnrolledUsers.Any(u => u.Id == user.Id)
                ? LisContextType.CourseSection
                : LisContextType.CourseTemplate;

            // Instance
            ltiRequest.ToolConsumerInstanceGuid = "~/".ToAbsoluteUrl();
            var title = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
            ltiRequest.ToolConsumerInstanceName = title == null ? null : title.Title;
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
            ltiRequest.UserId = user.Id;
            ltiRequest.UserName = user.UserName;
            ltiRequest.SetRoles(GetLtiRolesForUser(user));

            // Outcomes service
            if (course.EnrolledUsers.Any(u => u.Id == user.Id))
            {
                var outcomesUrl = UrlHelper.GenerateUrl("DefaultApi", null, "OutcomesApi",
                    new RouteValueDictionary { { "httproute", string.Empty } }, RouteTable.Routes,
                    request.RequestContext, false);
                Uri outcomesUri;
                if (Uri.TryCreate(request.Url, outcomesUrl, out outcomesUri))
                {
                    ltiRequest.LisOutcomeServiceUrl = outcomesUri.AbsoluteUri;
                }
                ltiRequest.LisResultSourcedId =
                    JsonConvert.SerializeObject(new LisResultSourcedId
                    {
                        AssignmentId = assignment.AssignmentId, 
                        UserId = user.Id
                    });
            }

            // Tool Consumer Profile service
            var profileUrl = UrlHelper.GenerateUrl("DefaultApi", null, "ToolConsumerProfileApi",
                new RouteValueDictionary { { "httproute", string.Empty } }, RouteTable.Routes,
                request.RequestContext, false);
            Uri profileUri;
            if (Uri.TryCreate(request.Url, profileUrl, out profileUri))
            {
                ltiRequest.ToolConsumerProfileUrl = profileUri.AbsoluteUri;
            }

            // Add the custom parameters. This consumer supports 3 special substitutions.
            var customParameters = assignment.CustomParameters;

            if (!string.IsNullOrWhiteSpace(customParameters))
            {
                ltiRequest.AddCustomParameters(customParameters);
            }

            return ltiRequest.GetLtiRequestViewModel(assignment.ConsumerSecret);
        }

        public static LtiRequestViewModel CreateContentItemSelectionRequestViewModel(HttpRequestBase request, ContentItemTool contentItemTool, Course course, ApplicationUser user, string returnUrl)
        {
            var ltiRequest = new LtiRequest(LtiConstants.ContentItemSelectionRequestLtiMessageType)
            {
                ConsumerKey = contentItemTool.ConsumerKey,
                Url = new Uri(contentItemTool.Url)
            };

            // Tool Consumer
            ltiRequest.ToolConsumerInfoProductFamilyCode = Assembly.GetExecutingAssembly().GetName().Name;
            ltiRequest.ToolConsumerInfoVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Context
            ltiRequest.ContextId = course.CourseId.ToString(CultureInfo.InvariantCulture);
            ltiRequest.ContextTitle = course.Name;
            ltiRequest.ContextType = course.EnrolledUsers.Any(u => u.Id == user.Id)
                ? LisContextType.CourseSection
                : LisContextType.CourseTemplate;
            ltiRequest.LisCourseSectionSourcedId = course.CourseId.ToString(CultureInfo.InvariantCulture);

            // Instance
            ltiRequest.ToolConsumerInstanceGuid = "~/".ToAbsoluteUrl();
            var title = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
            ltiRequest.ToolConsumerInstanceName = title == null ? null : title.Title;

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
            ltiRequest.UserId = user.Id;
            ltiRequest.UserName = user.UserName;
            ltiRequest.SetRoles(GetLtiRolesForUser(user));

            // Presentation
            ltiRequest.LaunchPresentationDocumentTarget = DocumentTarget.iframe;

            // Content Item Tool
            ltiRequest.AcceptMediaTypes = LtiConstants.LaunchMediaType; // Only accept LTI Link
            ltiRequest.AcceptMultiple = true;
            ltiRequest.AcceptPresentationDocumentTargets = "iframe";
            ltiRequest.AcceptUnsigned = false;
            ltiRequest.ContentItemReturnUrl = returnUrl;
            //ltiRequest.ContentItemServiceUrl = "about:blank";
            ltiRequest.Data = JsonConvert.SerializeObject(new ContentItemData
            {
                ContentItemToolId = contentItemTool.ContentItemToolId, 
                CourseId = course.CourseId
            });

            // Add the custom parameters.
            var customParameters = contentItemTool.CustomParameters;

            // This tool consumer supports 3 non-spec custom parameters
            if (!string.IsNullOrWhiteSpace(customParameters))
            {
                ltiRequest.AddCustomParameters(customParameters);
            }

            return ltiRequest.GetLtiRequestViewModel(contentItemTool.ConsumerSecret);
        }

        private static IList<Role> GetLtiRolesForUser(ApplicationUser user)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var roleManager = httpContext.GetOwinContext().Get<ApplicationRoleManager>();

            var roles = new List<Role>();
            foreach (var identityRole in user.Roles.Select(role => roleManager.FindById(role.RoleId)))
            {
                if (identityRole.Name.Equals(UserRoles.StudentRole)) roles.Add(Role.Learner);
                if (identityRole.Name.Equals(UserRoles.TeacherRole)) roles.Add(Role.Instructor);
            }
            
            return roles;
        }

        /// <summary>
        /// Get the company name stored in AssemblyInfo.
        /// </summary>
        public static string GetCompany()
        {
            var company = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyCompanyAttribute>().FirstOrDefault();
            return company == null ? "Sample" : company.Company;
        }

        /// <summary>
        /// Get the product name stored in AssemblyInfo.
        /// </summary>
        public static string GetProduct()
        {
            var product = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyProductAttribute>().FirstOrDefault();
            return product == null ? "Sample" : product.Product;
        }

        /// <summary>
        /// Get the application title stored in AssemblyInfo.
        /// </summary>
        public static string GetTitle()
        {
            var title = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyTitleAttribute>().FirstOrDefault();
            return title == null ? "Sample" : title.Title;
        }

        /// <summary>
        /// Get the application version stored in AssemblyInfo.
        /// </summary>
        public static string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version == null ? "0" : version.ToString();
        }
    }
}