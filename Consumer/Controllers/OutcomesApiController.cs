using Consumer.Lti;
using Consumer.Models;
using LtiLibrary.Outcomes;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System.Linq;
using System.Web;

namespace Consumer.Controllers
{
    /// <summary>
    /// Implements the LTI Basic Outcomes API.
    /// </summary>
    public class OutcomesApiController : OutcomesApiControllerBase
    {
        public OutcomesApiController() { }

        public OutcomesApiController(ApplicationUserManager userManager, ConsumerContext consumerContext)
        {
            UserManager = userManager;
            ConsumerContext = consumerContext;
        }

        public HttpContextBase HttpContext
        {
            get
            {
                // Since this WebApi is being hosted by the same ASP.NET application
                // as Consumer, I know the current OwinContext is available in HttpContextBase.
                return new HttpContextWrapper(System.Web.HttpContext.Current);
            }
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

        /// <summary>
        /// Return true if the request is authorized.
        /// </summary>
        /// <param name="ltiRequest">The LtiOucomesRequest to authorize.</param>
        /// <returns>True if the request is authorized by the Tool Consumer.</returns>
        protected override bool IsAuthorized(LtiOutcomesRequest ltiRequest)
        {
            // Every assignment has a consumer key and consumer secret. There is
            // nothing to prevent multiple providers from using the same key.
            var secrets = ConsumerContext.Assignments
                .Where(a => a.ConsumerKey == ltiRequest.ConsumerKey)
                .Select(assignment => assignment.ConsumerSecret)
                .Distinct()
                .ToList();

            return secrets.Any(secret => ltiRequest.GenerateSignature(secret).Equals(ltiRequest.Signature));
        }

        /// <summary>
        /// Delete the Score that corresponds to the result.
        /// </summary>
        /// <param name="lisResultSourcedId">The sourcedId of the LisResult to delete.</param>
        /// <returns>True if the Score was deleted.</returns>
        protected override bool DeleteResult(string lisResultSourcedId)
        {
            var model = GetScore(lisResultSourcedId);
            if (model.IsValid && model.Score != null)
            {
                ConsumerContext.Scores.Remove(model.Score);
                ConsumerContext.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fill the Result with corresponding Score data.
        /// </summary>
        /// <param name="lisResultSourcedId">The sourcedId of the LisResult to read.</param>
        /// <returns>True if the Score was found and the Result is valid.</returns>
        protected override LisResult ReadResult(string lisResultSourcedId)
        {
            var score = GetScore(lisResultSourcedId);
            var result = new LisResult();
            if (score.IsValid)
            {
                result.IsValid = true;
                result.Score = score.Score == null ? default(double?) : score.Score.DoubleValue;
                result.SourcedId = lisResultSourcedId;
            }
            else
            {
                result.IsValid = false;
            }
            return result;
        }

        /// <summary>
        /// Create or update a Score with the LTI result.
        /// </summary>
        /// <param name="result">The LTI result.</param>
        /// <returns>True if the Score was created or updated.</returns>
        protected override bool ReplaceResult(LisResult result)
        {
            var score = GetScore(result.SourcedId);
            if (score.IsValid)
            {
                if (score.Score == null)
                {
                    score.Score = CreateScore(result);
                    ConsumerContext.Scores.Add(score.Score);
                }
                if (result.Score.HasValue)
                {
                    score.Score.DoubleValue = result.Score.Value;
                }
                ConsumerContext.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create a new Score based on the Result that was sent.
        /// </summary>
        /// <param name="result">The result record that was sent in the request.</param>
        /// <returns>The Score based on the result.</returns>
        private static Score CreateScore(LisResult result)
        {
            var sourcedId = JsonConvert.DeserializeObject<LisResultSourcedId>(result.SourcedId);
            return new Score
                {
                    AssignmentId = sourcedId.AssignmentId,
                    UserId = sourcedId.UserId
                };
        }

        /// <summary>
        /// Return an existing Score based on the ressult.
        /// </summary>
        /// <param name="lisResultSourcedId">The sourcedId of the LisResult.</param>
        /// <returns>The existing Score or null if the doubleValue
        /// did not exist.</returns>
        private ReadScoreModel GetScore(string lisResultSourcedId)
        {
            var sourcedId = JsonConvert.DeserializeObject<LisResultSourcedId>(lisResultSourcedId);
            if (sourcedId == null)
            {
                return new ReadScoreModel { IsValid = false };
            }

            var assignment = ConsumerContext.Assignments.Find(sourcedId.AssignmentId);
            if (assignment == null)
            {
                return new ReadScoreModel { IsValid = false };
            }

            var user = UserManager.FindById(sourcedId.UserId);
            if (user == null)
            {
                return new ReadScoreModel { IsValid = false };
            }

            if (!assignment.Course.EnrolledUsers.Contains(user))
            {
                return new ReadScoreModel { IsValid = false };
            }

            var score = ConsumerContext.Scores.FirstOrDefault
                (s => s.AssignmentId == sourcedId.AssignmentId && s.UserId == sourcedId.UserId);

            return new ReadScoreModel { Score = score, IsValid = true };
        }
    }
}
