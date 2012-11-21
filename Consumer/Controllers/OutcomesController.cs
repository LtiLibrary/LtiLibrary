using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Consumer.Models;
using log4net;
using OAuth.Net.Common;
using OAuth.Net.Components;

namespace Consumer.Controllers
{
    /// <summary>
    /// Validate the OAuth Authorization header by checking the body hash
    /// and signature.
    /// </summary>
    public class OAuthAuthorizeAttribute : AuthorizeAttribute
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            if (request.Headers.Authorization == null)
            {
                log.Info("No Authorization header");
                return false;
            }

            if (!request.Headers.Authorization.Scheme.Equals(Constants.OAuthAuthScheme))
            {
                log.Info("Authorization scheme is not OAuth");
                return false;
            }

            using (var db = new ConsumerContext())
            {
                log.Debug("Authorization Params: " + request.Headers.Authorization.Parameter);

                var oauth_consumer_key = string.Empty;
                var oauth_signature = string.Empty;
                var oauthParameters = new OAuthParameters();

                // Parse the Authorization parameter and extract the signature

                foreach (var pair in request.Headers.Authorization.Parameter.Split(','))
                {
                    var keyValue = pair.Split('=');
                    var key = keyValue[0];
                    var value = HttpUtility.UrlDecode(keyValue[1].Trim('"'));

                    if (key.Equals("oauth_signature"))
                    {
                        oauth_signature = value;
                    }
                    else if (key.StartsWith("oauth_"))
                    {
                        if (key.Equals(Constants.ConsumerKeyParameter))
                            oauth_consumer_key = value;
                        oauthParameters.AdditionalParameters.Add(key, value);
                    }
                }

                // Compare the body hash to make sure the content was not
                // tampered with

                using (var sha1 = new SHA1CryptoServiceProvider())
                using (var task = request.Content.ReadAsStreamAsync())
                {
                    task.Wait(3000);
                    if (!task.IsCompleted)
                    {
                        log.Error("Timeout reading request content");
                        return false;
                    }

                    var hash = sha1.ComputeHash(task.Result);
                    var hash64 = Convert.ToBase64String(hash);
                    task.Result.Position = 0;

                    if (!hash64.Equals(oauthParameters.AdditionalParameters["oauth_body_hash"]))
                    {
                        log.Info("oauth_body_hash does not match");
                        return false;
                    }
                }

                var signatureProvider = new HmacSha1SigningProvider();
                var signatureBase = SignatureBase.Create("POST", request.RequestUri, oauthParameters);
                log.Debug("Signature base: " + signatureBase);

                // The key may be used by multiple providers, so scan them all
                // until a match is found

                foreach (var assignment in db.Assignments.Where(a => a.ConsumerKey == oauth_consumer_key))
                {
                    var signature = signatureProvider.ComputeSignature(signatureBase, assignment.Secret, string.Empty);
                    if (signature.Equals(oauth_signature))
                        return true;
                }

                log.Debug("OAuth signature does not match");
                return false;
            }
        }
    }

    public class OutcomesController : ApiController
    {
        private ConsumerContext db = new ConsumerContext();

        // POST api/outcomes

        [HttpPost, OAuthAuthorize]
        public imsx_POXEnvelopeType Post(imsx_POXEnvelopeType request)
        {
            imsx_POXEnvelopeType response;

            var requestHeader = request.imsx_POXHeader.Item as imsx_RequestHeaderInfoType;
            var requestBody = request.imsx_POXBody;

            if (requestBody.Item is deleteResultRequest)
            {
                var deleteRequest = requestBody.Item as deleteResultRequest;
                var deleteResponse = new deleteResultResponse();

                var score = GetScore(deleteRequest.resultRecord);
                if (score != null)
                {
                    db.Scores.Remove(score);
                    db.SaveChanges();
                }

                response = CreateResponse(requestHeader,
                    string.Format("Score for {0} is deleted",
                        deleteRequest.resultRecord.sourcedGUID.sourcedId
                        ));
                response.imsx_POXBody.Item = deleteResponse;
                return response;
            }
            else if (requestBody.Item is readResultRequest)
            {
                var readRequest = requestBody.Item as readResultRequest;
                var readResponse = new readResultResponse();

                var score = GetScore(readRequest.resultRecord);
                if (score == null)
                {
                    response = CreateResponse(requestHeader,
                        string.Format("Score for {0} not found",
                            readRequest.resultRecord.sourcedGUID.sourcedId
                            ));
                    response.imsx_POXBody.Item = readResponse;
                    var header = response.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
                    header.imsx_statusInfo.imsx_codeMinor = new imsx_CodeMinorFieldType[] {
                        new imsx_CodeMinorFieldType() {
                            imsx_codeMinorFieldName = "sourcedId",
                            imsx_codeMinorFieldValue = imsx_CodeMinorValueType.unknownobject
                        }};
                }
                else
                {
                    response = CreateResponse(requestHeader,
                        string.Format("Score for {0} is read",
                            readRequest.resultRecord.sourcedGUID.sourcedId
                            ));
                    readResponse.result = new ResultType();
                    readResponse.result.resultScore = new TextType();
                    readResponse.result.resultScore.language = "en-US";
                    readResponse.result.resultScore.textString = score.DecimalValue.ToString();
                    response.imsx_POXBody.Item = readResponse;
                }
                return response;
            }
            else if (requestBody.Item is replaceResultRequest)
            {
                var replaceRequest = requestBody.Item as replaceResultRequest;
                var replaceResponse = new replaceResultResponse();

                var score = GetScore(replaceRequest.resultRecord);
                if (score == null)
                {
                    score = CreateScore(replaceRequest.resultRecord);
                    db.Scores.Add(score);
                }
                score.DecimalValue = GetScoreValue(replaceRequest.resultRecord);
                db.SaveChanges();

                response = CreateResponse(requestHeader,
                    string.Format("Score for {0} is now {1}",
                        replaceRequest.resultRecord.sourcedGUID.sourcedId,
                        replaceRequest.resultRecord.result.resultScore.textString
                        ));
                response.imsx_POXBody.Item = replaceResponse;
                return response;
            }
            else
            {
                response = CreateResponse(requestHeader, "Request is not supported");
                var header = response.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
                header.imsx_statusInfo.imsx_codeMajor = imsx_CodeMajorType.unsupported;
                return response;
            }
        }

        private static imsx_POXEnvelopeType CreateResponse(imsx_RequestHeaderInfoType requestHeader, string description)
        {
            var response = new imsx_POXEnvelopeType();
            response.imsx_POXHeader = new imsx_POXHeaderType();
            response.imsx_POXHeader.Item = new imsx_ResponseHeaderInfoType();

            var item = response.imsx_POXHeader.Item as imsx_ResponseHeaderInfoType;
            item.imsx_version = imsx_GWSVersionValueType.V10;
            item.imsx_messageIdentifier = Guid.NewGuid().ToString();
            item.imsx_statusInfo = new imsx_StatusInfoType();

            var status = item.imsx_statusInfo;
            status.imsx_codeMajor = imsx_CodeMajorType.success;
            status.imsx_severity = imsx_SeverityType.status;
            status.imsx_description = description;
            status.imsx_messageRefIdentifier = requestHeader.imsx_messageIdentifier;

            response.imsx_POXBody = new imsx_POXBodyType();
            return response;
        }

        private Score CreateScore(ResultRecordType resultRecord)
        {
            int assignmentId;
            int userId;

            var ids = resultRecord.sourcedGUID.sourcedId.Split('-');
            int.TryParse(ids[0], out assignmentId);
            int.TryParse(ids[1], out userId);

            return new Score
                {
                    UserId = userId,
                    AssignmentId = assignmentId
                };
        }

        private Score GetScore(ResultRecordType resultRecord)
        {
            int assignmentId;
            int userId;

            var ids = resultRecord.sourcedGUID.sourcedId.Split('-');
            int.TryParse(ids[0], out assignmentId);
            int.TryParse(ids[1], out userId);

            return db.Scores.FirstOrDefault(
                s => s.AssignmentId == assignmentId && s.UserId == userId);
        }

        private decimal GetScoreValue(ResultRecordType resultRecord)
        {
            decimal value;
            decimal.TryParse(resultRecord.result.resultScore.textString, out value);
            return value;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
