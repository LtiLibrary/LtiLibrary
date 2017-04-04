using System.Collections.Generic;
using LtiLibrary.NetCore.ContentItems;
using LtiLibrary.NetCore.Lti1;
using LtiLibrary.NetCore.Tests.SimpleHelpers;
using Xunit;

namespace LtiLibrary.NetCore.Tests
{
    public class ContentItemTests
    {
        [Fact]
        public void LtiLinkContentItem_MatchesReferenceJson()
        {
            // From http://www.imsglobal.org/lti/services/ltiCIv1p0pd/ltiCIv1p0pd.html
            var ltiLink = new LtiLink
            {
                Icon = new Image
                {
                    Id = "https://www.server.com/path/animage.png",
                    Height = 50,
                    Width = 50
                },
                Text = "Read this section prior to your tutorial.",
                Title = "Week 1 reading",
                Custom = new Dictionary<string, string> { { "chapter", "12" }, { "section", "3" } }
            };

            JsonAssertions.AssertSameObjectJson(ltiLink, "LtiLinkContentItem");
        }

        [Fact]
        public void HyperlinkWithThumbnailContentItem_MatchesReferenceJson()
        {
            // From http://www.imsglobal.org/lti/services/ltiCIv1p0pd/ltiCIv1p0pd.html
            var contentItem = new ContentItem
            {
                MediaType = "text/html",
                PlacementAdvice = new ContentItemPlacement
                {
                    PresentationDocumentTarget = DocumentTarget.window,
                    WindowTarget = "_blank"
                },
                Thumbnail = new Image
                {
                    Id = "http://developers.imsglobal.org/images/imscertifiedsm.png",
                    Height = 184,
                    Width = 147
                },
                Title = "IMS catalog of certified products",
                Url = "http://imscatalog.org/"
            };

            JsonAssertions.AssertSameObjectJson(contentItem, "HyperlinkWithThumbnailContentItem");
        }

        [Fact]
        public void EmbeddedHtmlContentItem_MatchesReferenceJson()
        {
            // From http://www.imsglobal.org/lti/services/ltiCIv1p0pd/ltiCIv1p0pd.html
            var contentItem = new ContentItem
            {
                MediaType = "text/html",
                PlacementAdvice = new ContentItemPlacement
                {
                    PresentationDocumentTarget = DocumentTarget.embed
                },
                Text = "<p>IMS has a <a href=\"http://imscatalog.org/\">catalog of certified products</a> available on their website</p>"
            };

            JsonAssertions.AssertSameObjectJson(contentItem, "EmbeddedHtmlContentItem");
        }

        [Fact]
        public void EmbeddedImageContentItem_MatchesReferenceJson()
        {
            // From http://www.imsglobal.org/lti/services/ltiCIv1p0pd/ltiCIv1p0pd.html
            var contentItem = new ContentItem
            {
                MediaType = "image/png",
                PlacementAdvice = new ContentItemPlacement
                {
                    DisplayHeight = 184,
                    DisplayWidth = 147,
                    PresentationDocumentTarget = DocumentTarget.embed,
                    WindowTarget = "_blank"
                },
                Title = "IMS logo for certified products",
                Url = "http://developers.imsglobal.org/images/imscertifiedsm.png"
            };

            JsonAssertions.AssertSameObjectJson(contentItem, "EmbeddedImageContentItem");
        }
    }

}
