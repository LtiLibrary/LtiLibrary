using Consumer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Consumer.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        public CourseController() { }

        public CourseController(ApplicationUserManager userManager, ConsumerContext consumerContext)
        {
            UserManager = userManager;
            ConsumerContext = consumerContext;
        }

        private ConsumerContext _consumerContext;
        public ConsumerContext ConsumerContext
        {
            get
            {
                return _consumerContext ?? HttpContext.GetOwinContext().Get<ConsumerContext>();
            }
            private set
            {
                _consumerContext = value;
            }
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Course/
        public ActionResult Index()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            return View(ConsumerContext.Courses.ToList());
        }

        //
        // GET: /Course/Details/5
        public ActionResult Details(int id = 0)
        {
            var userId = User.Identity.GetUserId();
            var course = ConsumerContext.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            var scoredAssignments = new List<ScoredAssignmentModel>();
            foreach (var assignment in course.Assignments)
            {
                var scoredAssignment = new ScoredAssignmentModel(assignment);
                var score = ConsumerContext.Scores.FirstOrDefault(s =>
                    s.AssignmentId == assignment.AssignmentId &&
                    s.UserId == userId);
                scoredAssignment.Score = score == null ? null : score.DoubleValue.ToString(CultureInfo.InvariantCulture);
                scoredAssignment.UserId = userId;
                scoredAssignments.Add(scoredAssignment);
            }
            var model = new CourseViewModel
            {
                Course = course,
                ScoredAssignments = scoredAssignments,
            };
            ViewBag.UserId = userId;
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ContentItemTools(int courseid)
        {
            var model =
                ConsumerContext.ContentItemTools
                    .Where(t => !string.IsNullOrEmpty(t.Name))
                    .OrderBy(t => t.Name)
                    .Select(
                        t =>
                            new ContentItemToolViewModel
                            {
                                ContentItemToolId = t.ContentItemToolId,
                                CourseId = courseid,
                                Description = t.Description,
                                Name = t.Name
                            })
                    .ToList();
            return PartialView("_ContentItemToolsPartial", model);
        }

        [ChildActionOnly]
        public ActionResult CourseAssignment(int id)
        {
            var userId = User.Identity.GetUserId();
            var assignment = ConsumerContext.Assignments.Find(id);
            var score = ConsumerContext.Scores.FirstOrDefault(s =>
                s.AssignmentId == assignment.AssignmentId &&
                s.UserId == userId);
            var model = new ScoredAssignmentModel(assignment)
            {
                Score = score == null ? null : score.DoubleValue.ToString(CultureInfo.InvariantCulture),
                UserId = userId
            };
            return PartialView("_CourseAssignmentPartial", model);
        }

        [ChildActionOnly]
        public ActionResult CourseGradebook(int id)
        {
            var course = ConsumerContext.Courses.Find(id);
            var scores = new Dictionary<Tuple<string, int>, string>();
            foreach (var user in course.EnrolledUsers)
            {
                foreach (var assignment in course.Assignments)
                {
                    var score = ConsumerContext.Scores.FirstOrDefault(s =>
                        s.AssignmentId == assignment.AssignmentId &&
                        s.UserId == user.Id
                        );
                    scores.Add(
                        new Tuple<string, int>(user.Id, assignment.AssignmentId),
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
            var course = ConsumerContext.Courses.Find(courseId);
            var enrolled = course.EnrolledUsers.Count(u => u.Id == User.Identity.GetUserId()) > 0;
            var model = new CourseEnrollmentModel
            {
                CourseId = courseId,
                Enrolled = enrolled,
                UserId = User.Identity.GetUserId()
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
                var course = ConsumerContext.Courses.Find(model.CourseId);
                var user = UserManager.FindById(model.UserId);
                if (model.Enrolled)
                {
                    course.EnrolledUsers.Remove(user);
                }
                else
                {
                    course.EnrolledUsers.Add(user);
                }
                ConsumerContext.SaveChanges();
            }
            return RedirectToAction("Details", new { id = model.CourseId });
        }

        //
        // GET: /Course/Create
        [Authorize(Roles = "Teacher")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Course/Create
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public ActionResult Create(Course model)
        {
            if (ModelState.IsValid)
            {
                model.Instructor = UserManager.FindById(User.Identity.GetUserId());
                ConsumerContext.Courses.Add(model);
                ConsumerContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //
        // GET: /Course/Edit/5
        public ActionResult Edit(int id = 0)
        {
            var course = ConsumerContext.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            if (course.Instructor.Id != User.Identity.GetUserId())
            {
                return new HttpUnauthorizedResult();
            }
            return View(course);
        }

        //
        // POST: /Course/Edit/5
        [HttpPost]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                ConsumerContext.Entry(course).State = EntityState.Modified;
                ConsumerContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        //
        // GET: /Course/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Course course = ConsumerContext.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            if (course.Instructor.Id != User.Identity.GetUserId())
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
            var course = ConsumerContext.Courses.Find(id);
            foreach (var assignment in course.Assignments.ToList())
            {
                var assignment1 = assignment;
                var scores = ConsumerContext.Scores.Where(s => s.AssignmentId == assignment1.AssignmentId);
                foreach (var score in scores.ToList())
                {
                    ConsumerContext.Scores.Remove(score);
                }
                course.Assignments.Remove(assignment);
                ConsumerContext.Assignments.Remove(assignment);
            }
            foreach (var user in course.EnrolledUsers.ToList())
            {
                course.EnrolledUsers.Remove(user);
            }
            ConsumerContext.Courses.Remove(course);
            ConsumerContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}