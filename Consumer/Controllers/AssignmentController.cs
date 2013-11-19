using System.Data;
using System.Linq;
using System.Web.Mvc;
using Consumer.Lti;
using Consumer.Models;
using inBloomLibrary;

namespace Consumer.Controllers
{
    [Authorize]
    public class AssignmentController : Controller
    {
        private readonly ConsumerContext _db = new ConsumerContext();

        //
        // GET: /Assignment/Details/5

        public ActionResult Details(int id = 0)
        {
            Assignment assignment = _db.Assignments.Find(id);
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
                var course = _db.Courses.Find(model.CourseId);
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

                _db.Assignments.Add(assignment);
                _db.SaveChanges();

                if (!string.IsNullOrEmpty(course.inBloomSectionId))
                {
                    var assignmentModel = new inBloomLibrary.Models.Assignment
                    {
                        AssignmentId = assignment.AssignmentId,
                        ConsumerKey = assignment.ConsumerKey,
                        ConsumerSecret = assignment.ConsumerSecret,
                        CustomParameters = assignment.CustomParameters,
                        Description = assignment.Description,
                        Name = assignment.Name,
                        Url = assignment.Url
                    };
                    assignment.inBloomGradebookEntryId = inBloomApi.CreateGradebookEntry(
                        course.inBloomSectionId, 
                        assignmentModel);
                    _db.SaveChanges();
                }

                return RedirectToAction("Details", "Course", new { id = model.CourseId });
            }
            return View(model);
        }

        //
        // GET: /Assignment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var assignment = _db.Assignments.Find(id);
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
                var assignment = _db.Assignments.Find(model.AssignmentId);
                assignment.ConsumerKey = model.ConsumerKey;
                assignment.ConsumerSecret = model.ConsumerSecret;
                assignment.Course = _db.Courses.Find(model.CourseId);
                assignment.CustomParameters = model.CustomParameters;
                assignment.Description = model.Description;
                assignment.Name = model.Name;
                assignment.Url = model.Url;
                _db.Entry(assignment).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Details", "Course", new { id = model.CourseId });
            }
            return View(model);
        }

        //
        // GET: /Assignment/Delete/5

        public ActionResult Delete(int id = 0)
        {
            var assignment = _db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            var model = new DeleteAssignmentModel(assignment);
            model.DeleteInBloomGradebookEntry = false;

            return View(model);
        }

        //
        // POST: /Assignment/Delete/5

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, bool deleteInBloomGradebookEntry)
        {
            var assignment = _db.Assignments.Find(id);
            if (deleteInBloomGradebookEntry)
            {
                inBloomApi.DeleteGradebookEntry(assignment.inBloomGradebookEntryId);
            }

            var scores = _db.Scores.Where(s => s.AssignmentId == id);
            foreach (var score in scores.ToList())
            {
                _db.Scores.Remove(score);
            }
            var courseId = assignment.Course.CourseId;
            _db.Assignments.Remove(assignment);
            _db.SaveChanges();

            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        //
        // GET: /Assignment/Launch/5

        public ActionResult Launch(string returnUrl, int id = 0)
        {
            //return new RedirectResult("../LtiLaunch/2");
            var assignment = _db.Assignments.Find(id);
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

        public ActionResult LtiLaunch(int id = 0)
        {
            Assignment assignment = _db.Assignments.Find(id);
            
            return View(LtiUtility.CreateLtiRequest(assignment));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}