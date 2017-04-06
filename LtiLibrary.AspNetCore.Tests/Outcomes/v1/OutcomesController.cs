using LtiLibrary.AspNetCore.Outcomes.v1;
using LtiLibrary.NetCore.Outcomes.v1;

namespace LtiLibrary.AspNetCore.Tests.Outcomes.v1
{
    /// <summary>
    /// The OutcomesoApiController is hosted by the Tool Consumer and provides
    /// the functionality of the Outcomes Service API described in IMS LTI 1.1.
    /// </summary>
    public class OutcomesController : OutcomesControllerBase
    {
        // Simple "database" of scores for demonstration purposes
        private static LisResult _lisResult;

        /// <summary>
        /// Delete the score from the consumer database.
        /// </summary>
        /// <param name="lisResultSourcedId">The SourcedId of the score to delete.</param>
        /// <returns>True if the score is deleted.</returns>
        protected override bool DeleteResult(string lisResultSourcedId)
        {
            _lisResult = null;
            return true;
        }

        /// <summary>
        /// Read the score from the consumer database.
        /// </summary>
        /// <param name="lisResultSourcedId">The SourcedId of the score to read.</param>
        /// <returns>The LisResult representing the score.</returns>
        protected override LisResult ReadResult(string lisResultSourcedId)
        {
            if (_lisResult == null || !lisResultSourcedId.Equals(_lisResult.SourcedId))
            {
                return null;
            }
            return _lisResult;
        }

        /// <summary>
        /// Store the score in the consumer database.
        /// </summary>
        /// <param name="result">The LisResult to store.</param>
        /// <returns>True if the score is saved.</returns>
        protected override bool ReplaceResult(LisResult result)
        {
            if (_lisResult == null)
            {
                _lisResult = new LisResult();
            }
            _lisResult.Score = result.Score;
            _lisResult.SourcedId = result.SourcedId;
            return true;
        }
    }
}
