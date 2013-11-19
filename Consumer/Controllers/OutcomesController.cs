using System;
using System.Collections.Generic;
using System.Linq;
using Consumer.Models;
using LtiLibrary.Consumer;
using inBloomLibrary;

namespace Consumer.Controllers
{
    /// <summary>
    /// Implements the LTI Basic Outcomes API.
    /// </summary>
    public class OutcomesController : OutcomesControllerBase
    {
        private readonly ConsumerContext _db = new ConsumerContext();

        /// <summary>
        /// Returns a list of all the known consumer secrets for the given consumer key.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <returns>The list of consumer secrets.</returns>
        protected override IList<string> GetConsumerSecrets(string consumerKey)
        {
            // Every assignment has a consumer key and consumer secret.
            return _db.Assignments
                .Where(a => a.ConsumerKey == consumerKey)
                .Select(assignment => assignment.ConsumerSecret)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Delete the Score that corresponds to the result.
        /// </summary>
        /// <param name="result">The LTI result.</param>
        /// <returns>True if the Score was deleted.</returns>
        protected override bool DeleteResult(Result result)
        {
            var model = GetScore(result);
            if (model.IsValid && model.Score != null)
            {
                _db.Scores.Remove(model.Score);
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fill the Result with corresponding Score data.
        /// </summary>
        /// <param name="result">The LTI result.</param>
        /// <returns>True if the Score was found and the Result is valid.</returns>
        protected override bool ReadResult(Result result)
        {
            var score = GetScore(result);
            if (score.IsValid)
            {
                result.DoubleValue = score.Score == null ? default(double?) : score.Score.DoubleValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create or update a Score with the LTI result.
        /// </summary>
        /// <param name="result">The LTI result.</param>
        /// <returns>True if the Score was created or updated.</returns>
        protected override bool ReplaceResult(Result result)
        {
            var score = GetScore(result);
            if (score.IsValid)
            {
                if (score.Score == null)
                {
                    score.Score = CreateScore(result);
                    _db.Scores.Add(score.Score);
                }
                if (result.DoubleValue.HasValue)
                {
                    score.Score.DoubleValue = result.DoubleValue.Value;
                }
                _db.SaveChanges();

                // If this is an inBloom assignment, then send the doubleValue to inBloom.
                SetInBloomScore(score.Score.UserId, score.Score.AssignmentId, score.Score.DoubleValue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Send a Score to inBloom.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="assignmentId">The assignment ID.</param>
        /// <param name="doubleValue">The score as a decimal value (0.0 - 1.0).</param>
        private void SetInBloomScore(int userId, int assignmentId, double doubleValue)
        {
            var user = _db.Users.Find(userId);
            if (user == null || string.IsNullOrEmpty(user.SlcUserId))
            {
                return;
            }

            var assignment = _db.Assignments.Find(assignmentId);
            if (assignment == null || string.IsNullOrEmpty(assignment.inBloomGradebookEntryId))
            {
                return;
            }

            // LTI scores are between 0 and 1.0. SLC scores are between 0 and 100.
            var assignmentScore = new inBloomLibrary.Models.AssignmentScore
            {
                GradebookEntryId = assignment.inBloomGradebookEntryId,
                NumericGradeEarned = Convert.ToInt32(doubleValue * 100.0),
                StudentId = user.SlcUserId,
                TenantId = assignment.inBloomTenantId
            };

            inBloomApi.SetStudentGradebookEntryScore(assignmentScore);
        }

        /// <summary>
        /// Create a new Score based on the Result that was sent.
        /// </summary>
        /// <param name="result">The result record that was sent in the request.</param>
        /// <returns>The Score based on the result.</returns>
        private static Score CreateScore(Result result)
        {
            int assignmentId;
            int userId;

            var ids = result.SourcedGuid.Split('-');
            int.TryParse(ids[0], out assignmentId);
            int.TryParse(ids[1], out userId);

            return new Score
                {
                    UserId = userId,
                    AssignmentId = assignmentId
                };
        }

        /// <summary>
        /// Return an existing Score based on the ressult.
        /// </summary>
        /// <param name="result">The resultRecord which 
        /// specifies the assignment.</param>
        /// <returns>The existing Score or null if the doubleValue
        /// did not exist.</returns>
        private ReadScoreModel GetScore(Result result)
        {
            int assignmentId;
            int userId;

            var ids = result.SourcedGuid.Split('-');
            int.TryParse(ids[0], out assignmentId);
            int.TryParse(ids[1], out userId);

            var assignment = _db.Assignments.Find(assignmentId);
            if (assignment == null)
            {
                return new ReadScoreModel { IsValid = false };
            }

            var user = _db.Users.Find(userId);
            if (user == null)
            {
                return new ReadScoreModel { IsValid = false };
            }

            if (!assignment.Course.EnrolledUsers.Contains(user))
            {
                return new ReadScoreModel { IsValid = false };
            }

            var score = _db.Scores.FirstOrDefault
                (s => s.AssignmentId == assignmentId && s.UserId == userId);

            return new ReadScoreModel { Score = score, IsValid = true };
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
