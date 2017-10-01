using System.IO;
using HtmlAgilityPack;

namespace LtiLibrary.AspNetCore.Extensions
{
    /// <summary>
    /// <see cref="HtmlDocument"/> extension methods.
    /// </summary>
    public static class HtmlDocumentExtensions
    {
        /// <summary>
        /// Convert the <see cref="HtmlDocument"/> into plain text for use in LTI parameters
        /// that only accept plain text, such as <see cref="NetCore.Lti.v1.LtiRequest.ResourceLinkDescription"/>.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string ToPlainText(this HtmlDocument doc)
        {
            using (var sw = new StringWriter())
            {
                ConvertTo(doc.DocumentNode, sw);
                sw.Flush();
                return sw.ToString();
            }
        }

        private static void ConvertContentTo(HtmlNode node, TextWriter outText)
        {
            foreach (var subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outText);
            }
        }

        private static void ConvertTo(HtmlNode node, TextWriter outText)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Comment:
                    // don't output comments
                    break;

                case HtmlNodeType.Document:
                    ConvertContentTo(node, outText);
                    break;

                case HtmlNodeType.Text:
                    // script and style must not be output
                    var parentName = node.ParentNode.Name;
                    if (parentName == "script" || parentName == "style")
                        break;

                    // get text
                    var html = ((HtmlTextNode)node).Text;

                    // is it in fact a special closing node output as text?
                    if (HtmlNode.IsOverlappedClosingElement(html))
                        break;

                    // check the text is meaningful and not a bunch of whitespaces
                    if (html.Trim().Length > 0)
                    {
                        outText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        case "p":
                        case "li":
                            // treat paragraphs and lists as crlf
                            outText.Write("\r\n");
                            break;
                    }

                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outText);
                    }
                    break;
            }
        }
    }
}