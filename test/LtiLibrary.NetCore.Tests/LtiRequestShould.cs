using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.Lti.v1;
using LtiLibrary.NetCore.OAuth;
using Xunit;

namespace LtiLibrary.NetCore.Tests
{
    public class LtiRequestShould
    {
        #region Basic Launch Request Tests

        [Fact]
        public void ThrowException_IfMessageTypeIsMissing()
        {
            var request = new LtiRequest
            {
                Url = new Uri("http://lti.tools/test/tp.php"),
                ConsumerKey = "12345",
                ResourceLinkId = "launch"
            };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal("Invalid HTTP method: null.", ex.Message);
        }

        [Fact]
        public void ThrowException_IfMessageTypeIsUnknown()
        {
            var request = new LtiRequest("wrong")
            {
                Url = new Uri("http://lti.tools/test/tp.php"),
                ConsumerKey = "12345",
                ResourceLinkId = "launch"
            };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal($"Invalid {LtiConstants.LtiMessageTypeParameter}: wrong.", ex.Message);
        }

        [Fact]
        public void ThrowException_UrlIsMissing()
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                ConsumerKey = "12345",
                ResourceLinkId = "launch"
            };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal("Missing parameter(s): Url.", ex.Message);
        }

        [Fact]
        public void ThrowException_IfConsumerKeyIsMissing()
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                Url = new Uri("http://lti.tools/test/tp.php"),
                ResourceLinkId = "launch"
            };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal($"Missing parameter(s): {OAuthConstants.ConsumerKeyParameter}.", ex.Message);
        }

        [Fact]
        public void ThrowException_IfBasicLaunchRequestIsMissingResourceLinkId()
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                Url = new Uri("http://lti.tools/test/tp.php"),
                ConsumerKey = "12345"
            };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal($"Missing parameter(s): {LtiConstants.ResourceLinkIdParameter}.", ex.Message);
        }

        #endregion

        #region Content Item Request Tests

        [Fact]
        public void GenerateSignature_IfContentItemLaunchRequestHasAllRequiredFields()
        {
            var request =
                new LtiRequest(LtiConstants.ContentItemSelectionRequestLtiMessageType)
                {
                    Url = new Uri("http://lti.tools/test/tp.php"),
                    ConsumerKey = "12345",
                    AcceptMediaTypes = "text/html",
                    AcceptPresentationDocumentTargets = DocumentTarget.frame.ToString(),
                    ContentItemReturnUrl = "http://localhost/content"

                };
            request.Signature = request.SubstituteCustomVariablesAndGenerateSignature("secret");
        }

        [Fact]
        public void ThrowException_IfContentItemLaunchIsMissingAcceptMediaTypes()
        {
            var request =
                new LtiRequest(LtiConstants.ContentItemSelectionRequestLtiMessageType)
                {
                    Url = new Uri("http://lti.tools/test/tp.php"),
                    ConsumerKey = "12345",
                    AcceptPresentationDocumentTargets = DocumentTarget.frame.ToString(),
                    ContentItemReturnUrl = "http://localhost/content"

                };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal($"Missing parameter(s): {LtiConstants.AcceptMediaTypesParameter}.", ex.Message);
        }

        [Fact]
        public void ThrowException_IfContentItemLaunchIsMissingAcceptPresentationDocumentTargets()
        {
            var request =
                new LtiRequest(LtiConstants.ContentItemSelectionRequestLtiMessageType)
                {
                    Url = new Uri("http://lti.tools/test/tp.php"),
                    ConsumerKey = "12345",
                    AcceptMediaTypes = "text/html",
                    ContentItemReturnUrl = "http://localhost/content"
                };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal($"Missing parameter(s): {LtiConstants.AcceptPresentationDocumentTargetsParameter}.", ex.Message);
        }

        [Fact]
        public void ThrowException_IfContentItemLaunchIsMissingContentItemReturnUrl()
        {
            var request =
                new LtiRequest(LtiConstants.ContentItemSelectionRequestLtiMessageType)
                {
                    Url = new Uri("http://lti.tools/test/tp.php"),
                    ConsumerKey = "12345",
                    AcceptMediaTypes = "text/html",
                    AcceptPresentationDocumentTargets = DocumentTarget.frame.ToString()
                };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal($"Missing parameter(s): {LtiConstants.ContentItemReturnUrlParameter}.", ex.Message);
        }

        [Fact]
        public void ThrowException_IfContentItemLaunchUrlIsInvalid()
        {
            var request =
                new LtiRequest(LtiConstants.ContentItemSelectionRequestLtiMessageType)
                {
                    Url = new Uri("http://lti.tools/test/tp.php"),
                    ConsumerKey = "12345",
                    AcceptMediaTypes = "text/html",
                    AcceptPresentationDocumentTargets = DocumentTarget.frame.ToString(),
                    ContentItemReturnUrl = "w:r:o:n:g"
                };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal($"Invalid {LtiConstants.ContentItemReturnUrlParameter}: w:r:o:n:g.", ex.Message);
        }

        [Fact]
        public void GenerateSignature_IfContentItemLaunchResponsetHasAllRequiredFields()
        {
            var request =
                new LtiRequest(LtiConstants.ContentItemSelectionLtiMessageType)
                {
                    Url = new Uri("http://lti.tools/test/tp.php"),
                    ConsumerKey = "12345",
                    ContentItems = new ContentItem
                    {
                        MediaType = "text/html",
                        Text = "<p>Text</p>"
                    }.ToJsonLdString()
                };
            request.Signature = request.SubstituteCustomVariablesAndGenerateSignature("secret");
        }

        [Fact]
        public void ThrowException_IfContentItemResponseIsMissingContentItems()
        {
            var request =
                new LtiRequest(LtiConstants.ContentItemSelectionLtiMessageType)
                {
                    Url = new Uri("http://lti.tools/test/tp.php"),
                    ConsumerKey = "12345"
                };
            var ex = Assert.Throws<LtiException>(() => request.SubstituteCustomVariablesAndGenerateSignature("secret"));
            Assert.Equal($"Missing parameter(s): {LtiConstants.ContentItemPlacementParameter}.", ex.Message);
        }

        #endregion

        #region Signature Tests

        [Fact]
        public void GenerateSignature_IfBasicLaunchRequestHasAllRequiredFields()
        {
            var request =
                new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
                {
                    Url = new Uri("http://lti.tools/test/tp.php"),
                    ConsumerKey = "12345",
                    ResourceLinkId = "launch"
                };
            request.Signature = request.SubstituteCustomVariablesAndGenerateSignature("secret");
        }

        [Theory]
        [InlineData("HMAC-SHA1", "Im/RhYIEApfbsy+luuisvQqBjgs=")]
        [InlineData("HMAC-SHA256", "qQbfvgDf7WK/6+6XYCKlpu8klUMjH28NBk/U/CGOGEg=")]
        [InlineData("HMAC-SHA384", "FPZNr6ekXPwAI+B8N4Qvazgkq5mK7qnBvzRc52z/FxRfJA+hKggE2LBCqGVnkRud")]
        [InlineData("HMAC-SHA512", "ukswtvxdFC7RCVcmtxt/kvg6oE1v3zAJhtuNLRUnM3fjXIxMkg85uEq6Vz+fxLCEQ/YTPM9aBnsmEizyv3LxtQ==")]
        public void GenerateKnownSignature_GivenKnownLaunchParameters(string signatureMethod, string expectedSignature)
        {
            var request = new LtiRequest
            {
                CallBack = "about:blank",
                ConsumerKey = "12345",
                ContextId = "1219",
                ContextTitle = "docker",
                ContextType = ContextType.CourseTemplate,
                HttpMethod = HttpMethod.Post.Method,
                LaunchPresentationLocale = "en-US",
                LisPersonNameFamily = "Miller",
                LisPersonNameGiven = "Andy",
                LtiMessageType = "basic-lti-launch-request",
                LtiVersion = "LTI-1p0",
                Nonce = "c5c9781bfc3e4a1fb11b09ac119c860c",
                ResourceLinkId = "3280",
                ResourceLinkTitle = "docker",
                Roles = "Instructor,Learner",
                SignatureMethod = signatureMethod,
                Timestamp = 1492745602,
                ToolConsumerInfoProductFamilyCode = "Consumer",
                ToolConsumerInfoVersion = "1.5.0.0",
                ToolConsumerInstanceGuid = "http://consumer.azurewebsites.net/",
                ToolConsumerInstanceName = "Consumer Sample",
                Url = new Uri("http://localhost:32768/home/tool?one=one"),
                UserId = "98befcbc-117d-4328-9c70-93d198e44ddc",
                Version = "1.0"
            };
            var signature = request.GenerateSignature("secret");
            Assert.Equal(expectedSignature, signature);
        }

        [Theory]
        [InlineData("HMAC-SHA1", "1lsA7F7WPN48KzxVXDI25Lpam1E=")]
        [InlineData("HMAC-SHA256", "4KGg3xbB7Ke1FpIpQIEDvUuq5i2+PL+nFhU4BwM/jNw=")]
        [InlineData("HMAC-SHA384", "4qMYDlaEmS+c7SIXFHiJcuXsVgdKhtLS1pANiTCvHiWQK3m2aTG2fq9FNuAHZWbt")]
        [InlineData("HMAC-SHA512", "kh1W81jm0S2PnWH5DA73lYjJVe93EwfzDcnqiNim6kylvj67tnWPvSdm8R/zBsRZ7aZZxAyvZSSFKQwnlCBljA==")]
        public void GenerateKnownSignature_GivenKnownOutcomesParameters(string signatureMethod, string expectedSignature)
        {
            var request = new LtiRequest
            {
                BodyHash = "zxNf/lJoveI1hmN1ENbcdZdQ4Js=",
                ConsumerKey = "jisc.ac.uk",
                HttpMethod = HttpMethod.Post.Method,
                Nonce = "83e29bff47b7690a3f6b371c77526405",
                SignatureMethod = signatureMethod,
                Timestamp = 1493164209,
                Url = new Uri("http://lti.tools/test/tc-outcomes.php"),
                Version = "1.0"
            };
            var signature = request.GenerateSignature("secret");
            Assert.Equal(expectedSignature, signature);
        }

        #endregion

        #region Role Tests

        [Fact]
        public void SendFullUrnsForNonContextRoles()
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType);

            request.SetRoles(
                new List<Enum>
                {
                    ContextRole.Instructor,
                    InstitutionalRole.Administrator,
                    SystemRole.SysAdmin
                });

            Assert.Equal("Instructor,urn:lti:instrole:ims/lis/Administrator,urn:lti:sysrole:ims/lis/SysAdmin", request.Roles);
        }

        [Fact]
        public void CorrectlyInterpretRoles_GivenContextAndNonContextRolesString()
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                Roles = "Administrator,Instructor,urn:lti:instrole:ims/lis/Administrator,urn:lti:sysrole:ims/lis/SysAdmin"
            };

            var roles = request.GetRoles();
            Assert.Equal(4, roles.Count);
            Assert.Contains(ContextRole.Administrator, roles);
            Assert.Contains(ContextRole.Instructor, roles);
            Assert.Contains(InstitutionalRole.Administrator, roles);
            Assert.Contains(SystemRole.SysAdmin, roles);
        }

        #endregion

        #region Custom Parameter Tests

        [Theory]
        [InlineData("name", "value", "custom_name=value")]
        [InlineData(" name ", " value ", "custom_name=value")]
        [InlineData("Review:Chapter", "1.2.56", "custom_review_chapter=1.2.56")]
        [InlineData("Username", "$User.Username", "custom_username=amiller")]
        public void AddIndividualCustomParameters(string name, string value, string customParameters)
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                Url = new Uri("http://lti.tools/test/tp.php"),
                ConsumerKey = "12345",
                ResourceLinkId = "launch",
                UserName = "amiller"
            };
            request.AddCustomParameter(name, value);
            request.SubstituteCustomVariablesAndGenerateSignature("secret");
            Assert.Equal(customParameters, request.CustomParameters);
        }

        [Theory]
        [InlineData("name=value,Review:Chapter=1.2.56", "custom_name=value&custom_review_chapter=1.2.56")]
        [InlineData("name=value;Review:Chapter=1.2.56", "custom_name=value&custom_review_chapter=1.2.56")]
        [InlineData("name=value&Review:Chapter=1.2.56", "custom_name=value&custom_review_chapter=1.2.56")]
        [InlineData("name=value\nReview:Chapter=1.2.56", "custom_name=value&custom_review_chapter=1.2.56")]
        [InlineData("name=value\r\nReview:Chapter=1.2.56", "custom_name=value&custom_review_chapter=1.2.56")]
        [InlineData("name=value,Review:Chapter=1.2.56&last=first", "custom_name=value&custom_review_chapter=1.2.56&custom_last=first")]
        [InlineData("Username=$User.Username,Username=$User.Id", "custom_username=amiller&custom_username=12345")]
        public void AddMultipleCustomParameters(string customParameters, string expectedCustomParameters)
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                Url = new Uri("http://lti.tools/test/tp.php"),
                ConsumerKey = "12345",
                ResourceLinkId = "launch",
                UserId = "12345",
                UserName = "amiller"
            };
            request.AddCustomParameters(customParameters);
            request.SubstituteCustomVariablesAndGenerateSignature("secret");
            Assert.Equal(expectedCustomParameters, request.CustomParameters);
        }

        [Fact]
        public void TreatDuplicateCustomParameterNamesAsSeparateNameValue()
        {
            // HTML Form Data treats duplicate inputs as separate name/value pairs
            //
            // For example,
            //
            // <input type="text" name="firstname" value="Mickey" />
            // <input type="text" name="lastname" value="Mouse" />
            // <input type="text" name="lastname" value="Mouse" />
            //
            // Results in this Form Data when submitted
            //
            // firstname=Mickey&lastname=Mouse&lastname=Mouse
            //
            // LtiRequest custom parameters should treat the data the
            // same way.
            var request = new LtiRequest();
            request.AddCustomParameter("firstname", "Mickey");
            request.AddCustomParameter("lastname", "Mouse");
            request.AddCustomParameter("lastname", "Mouse");
            Assert.Equal("custom_firstname=Mickey&custom_lastname=Mouse&custom_lastname=Mouse", request.CustomParameters);
        }

        [Fact]
        public void TreatDuplicateCustomParametersWithSubstitutionSeparately()
        {
            var request = new LtiRequest(LtiConstants.BasicLaunchLtiMessageType)
            {
                Url = new Uri("http://lti.tools/test/tp.php"),
                ConsumerKey = "12345",
                ResourceLinkId = "launch",
                UserId = "12345",
                UserName = "amiller"
            };
            request.AddCustomParameter("Username", "$User.Username");
            request.AddCustomParameter("Username", "$User.Id");
            request.SubstituteCustomVariablesAndGenerateSignature("secret");
            Assert.Equal("custom_username=amiller&custom_username=12345", request.CustomParameters);
        }

        [Fact]
        public void ThrowAnException_IfCustomParameterNameIsNullOrWhitespace()
        {
            var request = new LtiRequest();
            Assert.Throws<ArgumentException>(() => request.AddCustomParameter(null, "value"));
            Assert.Throws<ArgumentException>(() => request.AddCustomParameter(string.Empty, "value"));
            Assert.Throws<ArgumentException>(() => request.AddCustomParameter(" ", "value"));
            Assert.Throws<ArgumentException>(() => request.AddCustomParameters("=value"));
            Assert.Throws<ArgumentException>(() => request.AddCustomParameters("name=value,=value"));
        }

        #endregion
    }
}
