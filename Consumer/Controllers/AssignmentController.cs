using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Consumer.Models;
using Consumer.Utility;
using OAuth.Net.Common;
using OAuth.Net.Components;
using WebMatrix.WebData;
using HtmlAgilityPack;

namespace Consumer.Controllers
{
    public class AssignmentController : Controller
    {
        private ConsumerContext db = new ConsumerContext();

        //
        // GET: /Assignment/

        public ActionResult Index()
        {
            return View(db.Assignments.ToList());
        }

        //
        // GET: /Assignment/Details/5

        public ActionResult Details(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        //
        // GET: /Assignment/Create

        [Authorize(Roles="Teacher")]
        public ActionResult Create()
        {
            ViewBag.LtiVersionId = new SelectList(db.LtiVersions, "LtiVersionId", "Name", LtiVersion.Version10);
            ViewBag.SharingScopeId = new SelectList(db.SharingScopes, "SharingScopeId", "Name", SharingScope.Private);
            return View();
        }

        //
        // POST: /Assignment/Create
        //
        // Note that ValidateInput is turned off so that teachers can use HTML in their descriptions

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateInput(false)]
        public ActionResult Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.UserId = WebSecurity.CurrentUserId;
                db.Assignments.Add(assignment);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.LtiVersionId = new SelectList(db.LtiVersions, "LtiVersionId", "Name", assignment.LtiVersionId);
            ViewBag.SharingScopeId = new SelectList(db.SharingScopes, "SharingScopeId", "Name", assignment.SharingScopeId);
            return View(assignment);
        }

        //
        // GET: /Assignment/Edit/5

        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.LtiVersionId = new SelectList(db.LtiVersions, "LtiVersionId", "Name", assignment.LtiVersionId);
            ViewBag.SharingScopeId = new SelectList(db.SharingScopes, "SharingScopeId", "Name", assignment.SharingScopeId);
            return View(assignment);
        }

        //
        // POST: /Assignment/Edit/5

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        [ValidateInput(false)]
        public ActionResult Edit(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.LtiVersionId = new SelectList(db.LtiVersions, "LtiVersionId", "Name", assignment.LtiVersionId);
            ViewBag.SharingScopeId = new SelectList(db.SharingScopes, "SharingScopeId", "Name", assignment.SharingScopeId);
            return View(assignment);
        }

        //
        // GET: /Assignment/Delete/5

        [Authorize(Roles = "Teacher")]
        public ActionResult Delete(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        //
        // POST: /Assignment/Delete/5

        [Authorize(Roles = "Teacher")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Assignment assignment = db.Assignments.Find(id);
            db.Assignments.Remove(assignment);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Assignment/Launch/5

        public ActionResult Launch(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
        
            // Public assignments are displayed on the front page even if the
            // user is not logged in. If the user is anonymous or if the 
            // assignment does not have an LTI key and secret defined, then
            // the Launch reverts to a simple redirect (GET). I'm curious to
            // see how providers handle this.

            if (Request.IsAuthenticated && assignment.IsLtiLink)
            {
                BuildLtiRequestViewBag(assignment);
                return View(assignment);
            }

            // If the user is not logged in or the assignment does not have an LTI
            // key and secret, then treat the launch as a simple link.
            return Redirect(assignment.Url);
        }

        private void BuildLtiRequestViewBag(Assignment assignment)
        {
            // Start with the basic and required parameters
            var parameters = BuildBaseLtiRequestData(assignment);

            // Add recommended and optional parameters
            AddOptionalParameters(assignment, parameters);

            // Add parameters that extend the basic launch data
            AddExtensionParameters(assignment, parameters);

            // Add version specific parameters
            if (assignment.LtiVersionId == LtiVersion.Version10)
                AddLti10Parameters(assignment, parameters);
            if (assignment.LtiVersionId == LtiVersion.Version11)
                AddLti11Parameters(assignment, parameters);

            // The LTI spec says to include the querystring parameters
            // when calculating the signature base string
            var uri = new Uri(assignment.Url);
            var querystring = HttpUtility.ParseQueryString(uri.Query);
            parameters.AdditionalParameters.Add(querystring);

            // Calculate the OAuth signature and send the data over to the view 
            // for rendering in the client browser. See Views/Assignment/Launch
            var signatureBase = SignatureBase.Create("POST", uri, parameters);
            var signatureProvider = new HmacSha1SigningProvider();

            // Now remove the querystring parameters so they are not sent twice
            // (once in the action URL and once in the form data)
            foreach (var name in querystring.AllKeys)
                parameters.AdditionalParameters.Remove(name);

            // Finally fill the ViewBag
            ViewBag.Signature = signatureProvider.ComputeSignature(signatureBase, assignment.Secret,
                string.Empty);
            ViewBag.Action = uri.ToString();           
            ViewBag.NameValues = HttpUtility.ParseQueryString(parameters.ToQueryStringFormat());
        }

        /// <summary>
        /// Calculate the data for a basic LTI 1.x request.
        /// </summary>
        /// <param name="assignment">The Assignment to be launched.</param>
        /// <returns>An OAuthParameters object which includes the required paremters
        /// for an LTI 1.x request.</returns>
        private OAuthParameters BuildBaseLtiRequestData(Assignment assignment)
        {
            const string lti_version = "LTI-1p0";
            const string lti_message_type = "basic-lti-launch-request";
            const string oauth_callback = "about:blank";
            const string oauth_signature_method = "HMAC-SHA1";
            const string oauth_version = "1.0";

            // First I calculate some values that I will need to sign the request
            // with OAuth.Net.
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
            var nonce = Guid.NewGuid().ToString("N");

            var parameters = new OAuthParameters();
            parameters.Callback = oauth_callback;
            parameters.ConsumerKey = assignment.ConsumerKey;
            parameters.Nonce = nonce;
            parameters.SignatureMethod = oauth_signature_method;
            parameters.Timestamp = timestamp;
            parameters.Version = oauth_version;

            // LTI Header: These identify the request as being an LTI request
            parameters.AdditionalParameters.Add("lti_message_type", lti_message_type);
            parameters.AdditionalParameters.Add("lti_version", lti_version);

            // Resource: These parameters identify the resource. In K-12, a resource is
            // equivalent to assignment and the resource_link_id must be unique to each
            // context_id (remember that context is equivalent to course or class). In
            // this sample, every user has their own course/class/context, so I simply
            // concatenate the class id with the assignment id to form the resource_link_id.
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            var course = new Course(user);
            parameters.AdditionalParameters.Add("resource_link_id", string.Format("{0}-{1}",
                course.Id, assignment.AssignmentId));
            // Note that the title is recommend, but not required.
            parameters.AdditionalParameters.Add("resource_link_title", assignment.Name);
            // The description is entirely optional, but if you do include it, it
            // must be converted to plain text.
            if (!string.IsNullOrWhiteSpace(assignment.Description))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(assignment.Description);

                parameters.AdditionalParameters.Add("resource_link_description",
                    doc.ToPlainText());
            }

            return parameters;
        }

        /// <summary>
        /// Add the optional parameters for an LTI 1.x request. Although these are
        /// optional according to the specification, they are required to pass the
        /// certification tests.
        /// </summary>
        /// <param name="assignment">The Assignment to be launched.</param>
        /// <param name="parameters">The partially filled OAuthParameters object
        /// that is being used to collect the launch data.</param>
        private void AddOptionalParameters(Assignment assignment, OAuthParameters parameters)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);

            // Tool Consumer: These identify this consumer to the provider. In K-12, tools
            // such as LMS and Portal systems are typically  purchased by the district and
            // shared by multiple schools in the district. My advice is to use the district
            // identity of the tool here (e.g. "Hillsboro School District LMS"). These
            // parameters are recommended.
            parameters.AdditionalParameters.Add("tool_consumer_instance_name",
                "LTI Consumer Sample");
            parameters.AdditionalParameters.Add("tool_consumer_instance_guid",
                "~/".ToAbsoluteUrl());

            // Context: These next parameters further identify where the request coming from.
            // "Context" can be thought of as the course or class. In this sample app, every
            // user automatically has their own "class" or list of assignment.
            var course = new Course(user);
            parameters.AdditionalParameters.Add("context_id", course.Id);
            parameters.AdditionalParameters.Add("context_label", course.Label);
            parameters.AdditionalParameters.Add("context_title", course.Title);
            parameters.AdditionalParameters.Add("context_type", course.LisType);

            // User: These parameters identify the user and their roles within the
            // context. These parameters are recommended.
            parameters.AdditionalParameters.Add("user_id", User.Identity.Name);
            parameters.AdditionalParameters.Add("roles", GetLtiRolesForUser());
            // Note that the potentially private information is suppressed if
            // the user chooses to hide it.
            if (user.SendEmail.GetValueOrDefault(true))
            {
                parameters.AdditionalParameters.Add("lis_person_contact_email_primary",
                    user.Email ?? string.Empty);
            }
            if (user.SendName.GetValueOrDefault(true))
            {
                parameters.AdditionalParameters.Add("lis_person_name_family",
                    user.LastName ?? string.Empty);
                parameters.AdditionalParameters.Add("lis_person_name_given",
                    user.FirstName ?? string.Empty);
            }

            // You can use launch_presentation_locale to send the preferred presentation
            // langauge, symbols, etc. I am sending the current UI culture (e.g. en-US).
            // This parameter is recommended.
            parameters.AdditionalParameters.Add("launch_presentation_locale",
                CultureInfo.CurrentUICulture.Name);
        }

        /// <summary>
        /// Add parameters that extend the basic launch data. These are not required
        /// for certification.
        /// </summary>
        /// <param name="assignment">The Assignment to be launched.</param>
        /// <param name="parameters">The partially filled OAuthParameters object
        /// that is being used to collect the launch data.</param>
        private void AddExtensionParameters(Assignment assignment, OAuthParameters parameters)
        {
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            var course = new Course(user);

            // To support K-12 tool providers that need to know the state, district
            // and school the class is in; let's add them if they are available.
            if (course.State != null)
            {
                parameters.AdditionalParameters.Add("ext_context_state_id",
                    course.State.StateId);
                parameters.AdditionalParameters.Add("ext_context_state_name",
                    course.State.Name);
            }
            if (course.District != null)
            {
                // LEA stands for Local Education Agency, which is the generic
                // name for a school district. We have two sourced id's for
                // districts: one from http://nces.ed.gov, and the other from
                // the state DOE.
                parameters.AdditionalParameters.Add("ext_context_lea_sourcedid",
                    string.Format("{0}:{1},{2}:{3}",
                        "nces.ed.gov", course.District.DistrictId,
                        "ext_context_state", course.District.StateDistrictId
                        ));
                parameters.AdditionalParameters.Add("ext_context_lea_name",
                    course.District.Name);
            }
            if (course.School != null)
            {
                // We also have two sourced id's for schools: one from
                // http://nces.ed.gov, and the other from the LEA.
                parameters.AdditionalParameters.Add("ext_context_school_sourcedid",
                    string.Format("{0}:{1},{2}:{3}",
                        "nces.ed.gov", course.School.SchoolId,
                        "ext_context_lea", course.School.DistrictSchoolId
                        ));
                parameters.AdditionalParameters.Add("ext_context_school_name",
                    course.School.Name);
            }
        }

        /// <summary>
        /// Add optional parameters that are specific to an LTI 1.0 request.
        /// </summary>
        /// <param name="assignment">The Assignment to be launched.</param>
        /// <param name="parameters">The partially filled OAuthParameters object
        /// that is being used to collect the data.</param>
        private void AddLti10Parameters(Assignment assignment, OAuthParameters parameters)
        {
            // LTI 1.0 does not include custom parameter substitution, so the custom parameter
            // values are added as-is.
            if (!string.IsNullOrWhiteSpace(assignment.CustomParameters))
            {
                var customParams = assignment.CustomParameters.Split(new[] { ",", "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var customParam in customParams)
                {
                    var namevalue = customParam.Split(new[] { "=" },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (namevalue.Length == 2)
                    {
                        // Note that per the LTI 1.x specs, custom parameter
                        // names must be lowercase letters or numbers. Any other
                        // character is replaced with an underscore.
                        var name = "custom_" + 
                            Regex.Replace(namevalue[0].ToLower(), "[^0-9a-zA-Z]", "_");
                        var value = namevalue[1];
                        parameters.AdditionalParameters.Add(name, value);
                    }
                }
            }
        }

        /// <summary>
        /// Add optional parameters that are specific to an LTI 1.1 request.
        /// </summary>
        /// <param name="assignment">The Assignment to be launched.</param>
        /// <param name="parameters">The partially filled OAuthParameters object
        /// that is being used to collect the data.</param>
        private void AddLti11Parameters(Assignment assignment, OAuthParameters parameters)
        {
            // LTI 1.1 does support custom parameter substitution
            if (!string.IsNullOrWhiteSpace(assignment.CustomParameters))
            {
                var customParams = assignment.CustomParameters.Split(new[] { ",", "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var customParam in customParams)
                {
                    var namevalue = customParam.Split(new[] { "=" },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (namevalue.Length == 2)
                    {
                        // Note that per the LTI 1.x specs, custom parameter
                        // names must be lowercase letters or numbers. Any other
                        // character is replaced with an underscore.
                        var name = "custom_" +
                            Regex.Replace(namevalue[0].ToLower(), "[^0-9a-zA-Z]", "_");
                        var value = SubstituteCustomValue(namevalue[1]);
                        parameters.AdditionalParameters.Add(name, value);
                    }
                }
            }

            // Basic Outcomes Service: These parameters tell the provider where to
            // send outcomes (if any) for this assignment.
            var urlHelper = new UrlHelper(Request.RequestContext);
            parameters.AdditionalParameters.Add("lis_outcome_service_url", 
                urlHelper.Action("Outcome", "Assignment", null, Request.Url.Scheme));
            parameters.AdditionalParameters.Add("lis_result_sourcedid", 
                assignment.AssignmentId.ToString());
        }

        /// <summary>
        /// Substitute known custom value tokens. Per the LTI 1.1 spec, unknown
        /// tokens are ignored.
        /// </summary>
        /// <param name="value">Custom value to scan.</param>
        /// <returns>Custom value with the known tokens replaced by their
        /// current value.</returns>
        private string SubstituteCustomValue(string value)
        {
            // LTI User variables
            var user = db.Users.Find(WebSecurity.CurrentUserId);
            value = Regex.Replace(value, "\\$User.id", 
                user.UserId.ToString(), RegexOptions.IgnoreCase);

            // LIS Person variables
            value = Regex.Replace(value, "\\$Person.sourcedId",
                user.UserId.ToString(), RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.full",
                user.FullName, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.family",
                user.LastName ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.given",
                user.FirstName ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.statepr",
                user.State == null ? 
                string.Empty :
                user.StateId, RegexOptions.IgnoreCase) ?? string.Empty;
            value = Regex.Replace(value, "\\$Person.address.country",
                "US", RegexOptions.IgnoreCase);
            if (user.SendEmail.GetValueOrDefault(true))
            {
                value = Regex.Replace(value, "\\$Person.email.primary",
                    user.Email, RegexOptions.IgnoreCase);
            }

            // LIS Course Section Variables
            var course = new Course(user);
            value = Regex.Replace(value, "\\$CourseSection.sourcedId",
                course.Id, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.label",
                course.Label, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.title",
                course.Title, RegexOptions.IgnoreCase);

            // State, district and school are common context values in K-12.
            value = Regex.Replace(value, "\\$CourseSection.location.stateId",
                course.State == null ? string.Empty : course.State.StateId, 
                RegexOptions.IgnoreCase);

            // The source of the following data is the NCES Elementary and 
            // Secondary School Information System (nces.ed.gov/ccd/elsi/).
            value = Regex.Replace(value, "\\$CourseSection.location.leaSourcedId",
                course.District == null ? string.Empty : course.District.DistrictId, 
                RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.location.seaLeaId",
                course.District == null ? string.Empty : course.District.StateDistrictId, 
                RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.location.leaName",
                course.District == null ? string.Empty : course.District.Name, 
                RegexOptions.IgnoreCase);

            value = Regex.Replace(value, "\\$CourseSection.location.schoolSourcedId",
                course.School == null ? string.Empty : course.School.SchoolId, 
                RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.location.leaSchoolId",
                course.School == null ? string.Empty : course.School.DistrictSchoolId, 
                RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$CourseSection.location.schoolName",
                course.School == null ? string.Empty : course.School.Name, 
                RegexOptions.IgnoreCase);

            return value;
        }

        private string GetLtiRolesForUser()
        {
            var roles = new List<string>();
            if (Roles.IsUserInRole(UserRoles.StudentRole)) roles.Add("Learner");
            if (Roles.IsUserInRole(UserRoles.TeacherRole)) roles.Add("Instructor");
            return string.Join(",", roles.ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}