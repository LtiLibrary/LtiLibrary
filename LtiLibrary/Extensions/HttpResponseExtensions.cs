using LtiLibrary.Lti1;
using System.Text;
using System.Web;

namespace LtiLibrary.Extensions
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Writes the LtiConsumerRequest to the HttpResponse so that the client browser
        /// executes the LTI launch request.
        /// </summary>
        /// <param name="response">The HttpResponse to write the launch request to.</param>
        /// <param name="request">The LtiConsumerRequest to write.</param>
        /// <param name="consumerSecret">The OAuth secret to use when signing the request.</param>
        public static void WriteLtiRequest(this HttpResponse response, LtiRequest request, string consumerSecret)
        {
            var model = request.GetLtiRequestViewModel(consumerSecret);
            var form = new StringBuilder();
            form.AppendLine("<html>");
            form.AppendLine("<head><title></title></head>");
            form.AppendLine("<body>");
            form.AppendFormat("<form method='post' action='{0}' id='form'>", model.Action).AppendLine();
            foreach (var key in model.Fields.AllKeys)
            {
                form.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key, model.Fields[key])
                    .AppendLine();
            }
            form.AppendFormat("<input type='hidden' name='oauth_signature' value='{0}' />", model.Signature).AppendLine();
            form.AppendLine("</form>");
            form.AppendLine("<script>");
            form.AppendLine(" document.getElementById('form').submit();");
            form.AppendLine("</script>");
            form.AppendLine("</body>");
            form.AppendLine("</html>");

            response.ContentType = "text/html";
            response.Write(form.ToString());
        }
    }
}
