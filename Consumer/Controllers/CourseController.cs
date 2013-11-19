using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Consumer.Models;
using WebMatrix.WebData;
using inBloomLibrary;

namespace Consumer.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ConsumerContext _db = new ConsumerContext();

        //
        // GET: /Course/

        public ActionResult Index()
        {
            ViewBag.inBloomAccount = inBloomSandboxClient.GetCurrentInBloomAccount();
            return View(_db.Courses.ToList());
        }

        //
        // GET: /Course/Details/5

        public ActionResult Details(int id = 0)
        {
            var course = _db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            var scoredAssignments = new List<ScoredAssignmentModel>();
            foreach (var assignment in course.Assignments)
            {
                var scoredAssignment = new ScoredAssignmentModel(assignment);
                var score = _db.Scores.FirstOrDefault(s => 
                    s.AssignmentId == assignment.AssignmentId && 
                    s.UserId == WebSecurity.CurrentUserId);
                scoredAssignment.Score = score == null ? null : score.DoubleValue.ToString(CultureInfo.InvariantCulture);
                scoredAssignments.Add(scoredAssignment);
            }
            ViewBag.ScoredAssignments = scoredAssignments;
            return View(course);
        }

        [ChildActionOnly]
        public ActionResult CourseAssignment(int id)
        {
            var assignment = _db.Assignments.Find(id);
            var score = _db.Scores.FirstOrDefault(s =>
                s.AssignmentId == assignment.AssignmentId &&
                s.UserId == WebSecurity.CurrentUserId
                );
            var model = new ScoredAssignmentModel(assignment)
            {
                Score = score == null ? null : score.DoubleValue.ToString(CultureInfo.InvariantCulture)
            };
            return PartialView("_CourseAssignmentPartial", model);
        }

        [ChildActionOnly]
        public ActionResult CourseGradebook(int id)
        {
            var course = _db.Courses.Find(id);
            var scores = new Dictionary<Tuple<int, int>, string>();
            foreach (var user in course.EnrolledUsers)
            {
                foreach (var assignment in course.Assignments)
                {
                    var score = _db.Scores.FirstOrDefault(s =>
                        s.AssignmentId == assignment.AssignmentId &&
                        s.UserId == user.UserId
                        );
                    scores.Add(
                        new Tuple<int, int>(user.UserId, assignment.AssignmentId), 
                        score == null ? null : score.DoubleValue.ToString(CultureInfo.InvariantCulture));
                }
            }
            var model = new CourseGradebookModel
            {
                Assignments = course.Assignments,
                EnrolledUsers = course.EnrolledUsers,
                Scores = scores
            };
            return PartialView("_CourseGradebookPartial", model);
        }

        //
        // GET: /Course/Enroll

        [ChildActionOnly]
        public ActionResult Enroll(int courseId)
        {
            var course = _db.Courses.Find(courseId);
            var enrolled = course.EnrolledUsers.Count(u => u.UserId == WebSecurity.CurrentUserId) > 0;
            var model = new CourseEnrollmentModel
            {
                CourseId = courseId,
                Enrolled = enrolled,
                UserId = WebSecurity.CurrentUserId
            };
            return PartialView("_EnrollPartial", model);
        }

        //
        // POST: /Course/Enroll/

        [HttpPost]
        public ActionResult Enroll(CourseEnrollmentModel model)
        {
            if (ModelState.IsValid)
            {
                var course = _db.Courses.Find(model.CourseId);
                var user = _db.Users.Find(model.UserId);
                if (model.Enrolled)
                {
                    course.EnrolledUsers.Remove(user);
                }
                else
                {
                    course.EnrolledUsers.Add(user);
                }
                _db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = model.CourseId });
        }

        //
        // GET: /Course/Create

        [Authorize(Roles = "Teacher")]
        public ActionResult Create()
        {
            var model = new CreateEditCourseModel();
            model.AvailableDistricts = GetAvailableDistricts(model.StateId, model.DistrictId);
            model.AvailableSchools = GetAvailableSchools(model.DistrictId, model.SchoolId);
            model.AvailableStates = GetAvailableStates(model.StateId);
            return View(model);
        }

        //
        // POST: /Course/Create

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult Create(CreateEditCourseModel model)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    Name = model.Name,
                    Instructor = _db.Users.Find(WebSecurity.CurrentUserId),
                    State = _db.States.Find(model.StateId),
                    District = _db.Districts.Find(model.DistrictId),
                    School = _db.Schools.Find(model.SchoolId)
                };

                _db.Courses.Add(course);
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = course.CourseId });
            }

            model.AvailableDistricts = GetAvailableDistricts(model.StateId, model.DistrictId);
            model.AvailableSchools = GetAvailableSchools(model.DistrictId, model.SchoolId);
            model.AvailableStates = GetAvailableStates(model.StateId);
            return View(model);
        }

        //
        // GET: /Course/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var course = _db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            if (course.Instructor.UserId != WebSecurity.CurrentUserId)
            {
                return new HttpUnauthorizedResult();
            }

            var model = new CreateEditCourseModel(course);
            model.AvailableDistricts = GetAvailableDistricts(model.StateId, model.DistrictId);
            model.AvailableSchools = GetAvailableSchools(model.DistrictId, model.SchoolId);
            model.AvailableStates = GetAvailableStates(model.StateId);
            return View(model);
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        public ActionResult Edit(CreateEditCourseModel model)
        {
            if (ModelState.IsValid)
            {
                var course = _db.Courses.
                    Where(c => c.CourseId == model.CourseId).
                    Include(c => c.State).
                    Include(c => c.District).
                    Include(c => c.School).
                    First();

                course.Name = model.Name;
                course.State = _db.States.Find(model.StateId);
                course.District = _db.Districts.Find(model.DistrictId);
                course.School = _db.Schools.Find(model.SchoolId);

                _db.Entry(course).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = course.CourseId });
            }

            model.AvailableDistricts = GetAvailableDistricts(model.StateId, model.DistrictId);
            model.AvailableSchools = GetAvailableSchools(model.DistrictId, model.SchoolId);
            model.AvailableStates = GetAvailableStates(model.StateId);
            return View(model);
        }

        //
        // GET: /Course/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Course course = _db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            if (course.Instructor.UserId != WebSecurity.CurrentUserId)
            {
                return new HttpUnauthorizedResult();
            }
            return View(course);
        }

        //
        // POST: /Course/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var course = _db.Courses.Find(id);
            foreach (var assignment in course.Assignments.ToList())
            {
                var assignment1 = assignment;
                var scores = _db.Scores.Where(s => s.AssignmentId == assignment1.AssignmentId);
                foreach (var score in scores.ToList())
                {
                    _db.Scores.Remove(score);
                }
                course.Assignments.Remove(assignment);
                _db.Assignments.Remove(assignment);
            }
            foreach (var user in course.EnrolledUsers.ToList())
            {
                course.EnrolledUsers.Remove(user);
            }
            _db.Courses.Remove(course);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Imports the SLC course sections of the SLC user.
        /// </summary>
        /// <example>
        /// GET /Course/ImportSlcCourses
        /// </example>
        public ActionResult ImportInBloomSections()
        {
            var sections = inBloomApi.GetSectionsForTeacher().
                OrderBy(c => c.UniqueSectionCode);
            return View(sections.ToList());
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult ImportInBloomSection(string sectionId, string name)
        {
            var course = _db.Courses.SingleOrDefault(c => c.inBloomSectionId == sectionId);

            var model = new ImportInBloomSectionModel
            {
                CourseId = course == null ? (int?)null : course.CourseId,
                Name = name,
                SectionId = sectionId
            };

            return PartialView("_ImportInBloomSectionPartial", model);
        }

        [HttpPost]
        public ActionResult ImportInBloomSection(ImportInBloomSectionModel model)
        {
            if (ModelState.IsValid)
            {
                inBloomLibrary.Models.Section section = inBloomApi.GetSection(model.SectionId);
                if (model.CourseId.HasValue)
                {
                    // TODO: Update the course
                }
                else
                {
                    // Create the course
                    var course = new Course
                    {
                        Name = section.UniqueSectionCode,
                        Instructor = _db.Users.Find(WebSecurity.CurrentUserId),
                        inBloomSectionId = model.SectionId
                    };
                    _db.Courses.Add(course);

                    // Store the tenantId with each assignment so that the LTI
                    // outcomes can be sent back to the SLC when they arrive days
                    // later
                    var tenantId = inBloomSandboxClient.GetCurrentInBloomAccount().TenantId;

                    foreach (var entry in section.GradebookEntries)
                    {
                        var assignment = new Assignment
                        {
                            Description = entry.Description,
                            inBloomGradebookEntryId = entry.Id
                        };
                        if (entry.Custom != null)
                        {
                            assignment.ConsumerKey = entry.Custom.ConsumerKey;
                            assignment.ConsumerSecret = entry.Custom.ConsumerSecret;
                            assignment.CustomParameters = entry.Custom.CustomParameters;
                            assignment.Name = entry.Custom.Name;
                            assignment.inBloomTenantId = tenantId;
                            assignment.Url = entry.Custom.Url;
                        }

                        course.Assignments.Add(assignment);
                    }

                    foreach (var student in section.Students)
                    {
                        var user = _db.Users.SingleOrDefault(u => u.SlcUserId == student.Id);
                        if (user == null)
                        {
                            var username = student.StateId;
                            var password = student.StateId;

                            WebSecurity.CreateUserAndAccount(username, password);
                            Roles.AddUserToRole(username, UserRoles.StudentRole);
                            
                            var userId = WebSecurity.GetUserId(username);
                            user = _db.Users.Find(userId);
                            user.SendEmail = true;
                            user.SendName = true;
                        }
                        user.Email = student.Email == null || student.Email.Length == 0 ? null : student.Email[0].EmailAddress;
                        user.FirstName = student.Name.FirstName;
                        user.LastName = student.Name.LastName;
                        user.SlcUserId = student.Id;

                        if (course.EnrolledUsers.Count(u => u.UserId == user.UserId) == 0)
                        {
                            course.EnrolledUsers.Add(user);
                        }
                    }
                }
                _db.SaveChanges();
            }
            return RedirectToAction("ImportInBloomSections");
        }

        private SelectList GetAvailableDistricts(string stateId = null, string districtId = null)
        {
            if (string.IsNullOrEmpty(stateId))
            {
                return new SelectList(new SelectListItem[] { });
            }
            return new SelectList(
                _db.Districts.Where(i => i.StateId.Equals(stateId)).OrderBy(i => i.Name),
                "DistrictId", "Name", districtId
                );
        }

        private SelectList GetAvailableSchools(string districtId = null, string schoolId = null)
        {
            if (string.IsNullOrEmpty(districtId))
            {
                return new SelectList(new SelectListItem[] { });
            }
            return new SelectList(
                _db.Schools.Where(i => i.DistrictId.Equals(districtId)).OrderBy(i => i.Name),
                "SchoolId", "Name", schoolId
                );
        }

        private SelectList GetAvailableStates(string stateId = null)
        {
            return new SelectList(_db.States.ToList(), "StateId", "Name", stateId);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}