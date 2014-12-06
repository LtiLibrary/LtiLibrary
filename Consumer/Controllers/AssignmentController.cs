using Consumer.Lti;
using Consumer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Consumer.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        public AssignmentController() {}

        public AssignmentController(ApplicationUserManager userManager, ConsumerContext consumerContext)
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
        // GET: /Assignment/Details/5
        public ActionResult Details(int id = 0)
        {
            Assignment assignment = ConsumerContext.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        //
        // GET: /Assignment/Create
        public ActionResult Create(int courseId)
        {
            var model = new CreateEditAssignmentModel
            {
                CourseId = courseId
            };

            return View(model);
        }

        //
        // POST: /Assignment/Create
        //
        // Note that ValidateInput is turned off so that teachers can use HTML in their descriptions
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(CreateEditAssignmentModel model)
        {
            if (ModelState.IsValid)
            {
                var course = ConsumerContext.Courses.Find(model.CourseId);
                var assignment = new Assignment
                {
                    ConsumerKey = model.ConsumerKey,
                    ConsumerSecret = model.ConsumerSecret,
                    Course = course,
                    CustomParameters = model.CustomParameters,
                    Description = model.Description,
                    Name = model.Name,
                    Url = model.Url
                };

                ConsumerContext.Assignments.Add(assignment);
                ConsumerContext.SaveChanges();

                return RedirectToAction("Details", "Course", new { id = model.CourseId });
            }
            return View(model);
        }

        //
        // GET: /Assignment/Edit/5
        public ActionResult Edit(int id = 0)
        {
            var assignment = ConsumerContext.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            var model = new CreateEditAssignmentModel(assignment);
            return View(model);
        }

        //
        // POST: /Assignment/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(CreateEditAssignmentModel model)
        {
            if (ModelState.IsValid)
            {
                var assignment = ConsumerContext.Assignments.Find(model.AssignmentId);
                assignment.ConsumerKey = model.ConsumerKey;
                assignment.ConsumerSecret = model.ConsumerSecret;
                assignment.Course = ConsumerContext.Courses.Find(model.CourseId);
                assignment.CustomParameters = model.CustomParameters;
                assignment.Description = model.Description;
                assignment.Name = model.Name;
                assignment.Url = model.Url;
                ConsumerContext.Entry(assignment).State = EntityState.Modified;
                ConsumerContext.SaveChanges();
                return RedirectToAction("Details", "Course", new { id = model.CourseId });
            }
            return View(model);
        }

        //
        // GET: /Assignment/Delete/5
        public ActionResult Delete(int id = 0)
        {
            var assignment = ConsumerContext.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(new DeleteAssignmentModel(assignment));
        }

        //
        // POST: /Assignment/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var assignment = ConsumerContext.Assignments.Find(id);
            var scores = ConsumerContext.Scores.Where(s => s.AssignmentId == id);
            foreach (var score in scores.ToList())
            {
                ConsumerContext.Scores.Remove(score);
            }
            var courseId = assignment.Course.CourseId;
            ConsumerContext.Assignments.Remove(assignment);
            ConsumerContext.SaveChanges();

            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        //
        // GET: /Assignment/Launch/5
        public ActionResult Launch(string returnUrl, int id = 0)
        {
            //return new RedirectResult("../LtiLaunch/2");
            var assignment = ConsumerContext.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }

            var model = new LaunchModel
            { 
                AssignmentId = id,
                AssignmentTitle = assignment.Name,
                CourseTitle = assignment.Course.Name,
                IsLtiLink = assignment.IsLtiLink,
                ReturnUrl = returnUrl,
                Url = assignment.Url
            };
            return View(model);
        }

        /// <summary>
        /// Form a basic LTI launch request for the browser to POST.
        /// </summary>
        /// <param name="id">The assignment ID to launch.</param>
        /// <returns>A form post for the browser to execute.</returns>
        public ActionResult LtiLaunch(int id = 0)
        {
            var assignment = ConsumerContext.Assignments.Find(id);
            var user = UserManager.FindById(User.Identity.GetUserId());
            
            return View(LtiUtility.CreateBasicLaunchRequestViewModel(Request, assignment, user));
        }
    }
}