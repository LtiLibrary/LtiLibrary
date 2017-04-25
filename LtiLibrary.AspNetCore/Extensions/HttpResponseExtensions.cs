using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using LtiLibrary.NetCore.Lti1;

namespace LtiLibrary.AspNetCore.Extensions
{
    /// <summary>
    /// <see cref="HttpResponse"/> extension methods.
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Writes the LtiConsumerRequest to the HttpResponse so that the client browser
        /// executes the LTI launch request.
        /// </summary>
        /// <param name="response">The HttpResponse to write the launch request to.</param>
        /// <param name="request">The LtiConsumerRequest to write.</param>
        /// <param name="consumerSecret">The OAuth secret to use when signing the request.</param>
        public static async Task WriteLtiRequest(this HttpResponse response, LtiRequest request, string consumerSecret)
        {
            request.Signature = request.SubstituteCustomVariablesAndGenerateSignature(consumerSecret);
            var form = new StringBuilder();
            form.AppendLine("<html>");
            form.AppendLine("<head><title></title></head>");
            form.AppendLine("<body>");
            form.AppendFormat("<form method='post' action='{0}' id='form'>", request.Url.AbsoluteUri).AppendLine();
            foreach (var key in request.Parameters.AllKeys)
            {
                form.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key, request.Parameters[key])
                    .AppendLine();
            }
            form.AppendLine("</form>");
            form.AppendLine("<script>");
            form.AppendLine(" document.getElementById('form').submit();");
            form.AppendLine("</script>");
            form.AppendLine("</body>");
            form.AppendLine("</html>");

            response.ContentType = "text/html";
            await response.WriteAsync(form.ToString());
        }
    }
}
