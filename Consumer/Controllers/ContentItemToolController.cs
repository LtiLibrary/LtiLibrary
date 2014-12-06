using Consumer.Lti;
using Consumer.Models;
using LtiLibrary.Common;
using LtiLibrary.ContentItems;
using LtiLibrary.Lti1;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Consumer.Controllers
{
    public class ContentItemToolController : Controller
    {
        public ContentItemToolController() { }

        public ContentItemToolController(ApplicationUserManager userManager, ConsumerContext consumerContext)
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

        // GET: /ContentItemTool/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ContentItemTool/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ContentItemToolId,ConsumerKey,ConsumerSecret,CustomParameters,Description,Name,Url")] ContentItemTool contentitemtool)
        {
            if (ModelState.IsValid)
            {
                contentitemtool.Owner = UserManager.FindById(User.Identity.GetUserId());
                ConsumerContext.ContentItemTools.Add(contentitemtool);
                ConsumerContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contentitemtool);
        }

        // GET: /ContentItemTool/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentItemTool contentitemtool = ConsumerContext.ContentItemTools.Find(id);
            if (contentitemtool == null)
            {
                return HttpNotFound();
            }
            return View(contentitemtool);
        }

        // POST: /ContentItemTool/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            ContentItemTool contentitemtool = ConsumerContext.ContentItemTools.Find(id);
            ConsumerContext.ContentItemTools.Remove(contentitemtool);
            ConsumerContext.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /ContentItemTool/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentItemTool contentitemtool = ConsumerContext.ContentItemTools.Find(id);
            if (contentitemtool == null)
            {
                return HttpNotFound();
            }
            return View(contentitemtool);
        }

        // GET: /ContentItemTool/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentItemTool contentitemtool = ConsumerContext.ContentItemTools.Find(id);
            if (contentitemtool == null)
            {
                return HttpNotFound();
            }
            return View(contentitemtool);
        }

        // POST: /ContentItemTool/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ContentItemToolId,ConsumerKey,ConsumerSecret,CustomParameters,Description,Name,Url")] ContentItemTool contentitemtool)
        {
            if (ModelState.IsValid)
            {
                ConsumerContext.Entry(contentitemtool).State = EntityState.Modified;
                ConsumerContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contentitemtool);
        }

        // GET: /ContentItemTool/
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            return View(ConsumerContext.ContentItemTools.ToList());
        }

        [Authorize]
        public ActionResult Launch(int contentItemToolId, int courseId)
        {
            var contentItemTool = ConsumerContext.ContentItemTools.Find(contentItemToolId);
            if (contentItemTool == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid content tool id" });
            }

            var course = ConsumerContext.Courses.Find(courseId);
            if (course == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid course id" });
            }

            var user = UserManager.FindById(User.Identity.GetUserId());
            var returnUrl = UrlHelper.GenerateUrl("Default", "PlaceContentItem", "ContentItemTool", null, RouteTable.Routes,
                Request.RequestContext, true);
            Uri returnUri;
            if (Uri.TryCreate(Request.Url, returnUrl, out returnUri))
            {
                returnUrl = returnUri.AbsoluteUri;
            }
            return View(LtiUtility.CreateContentItemSelectionRequestViewModel(Request, contentItemTool, course, user, returnUrl));
        }

        [HttpPost]
        [ValidateInput(false)]
        [LtiAuthorize(Order = 0)]
        [Authorize(Order = 1)]
        public ActionResult PlaceContentItem(LtiRequest model)
        {
            var ltiMessageType = model.LtiMessageType;
            if (!string.IsNullOrEmpty(ltiMessageType) &&
                !ltiMessageType.Equals(LtiConstants.ContentItemSelectionResponseLtiMessageType))
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Unknown LTI message" });
            }

            var ltiResponse = (IContentItemSelectionResponse)model;
            var data = JsonConvert.DeserializeObject<ContentItemData>(ltiResponse.Data);

            var contentItemTool = ConsumerContext.ContentItemTools.Find(data.ContentItemToolId);
            if (contentItemTool == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid content item tool id" });
            }

            var course = ConsumerContext.Courses.Find(data.CourseId);
            if (course == null)
            {
                return RedirectToAction("BadRequest", "Error", new { error = "Invalid course id" });
            }

            var contentItems = JsonConvert.DeserializeObject<ContentItemPlacementResponse>(ltiResponse.ContentItems);
            foreach (var contentItemPlacement in contentItems.Graph)
            {
                if (contentItemPlacement.PlacementOf.Type == ContentItemType.LtiLink)
                {
                    var placement = contentItemPlacement.PlacementOf as ILtiLink;
                    var custom = new StringBuilder();
                    foreach (var key in placement.Custom.Keys)
                    {
                        custom.AppendFormat("{0}={1}\n", key, placement.Custom[key]);
                    }
                    var assignment = new Assignment
                    {
                        ConsumerKey = contentItemTool.ConsumerKey,
                        Course = course,
                        ConsumerSecret = contentItemTool.ConsumerSecret,
                        CustomParameters = custom.ToString(),
                        Description = placement.Text,
                        Name = placement.Title,
                        Url = placement.Id ?? contentItemTool.Url
                    };
                    ConsumerContext.Assignments.Add(assignment);
                }
            }
            ConsumerContext.SaveChanges();

            return RedirectToAction("Details", "Course", new { id = data.CourseId });
        }
    }
}
