using System;
using System.IO;
using System.Web;

namespace SimpleLti12
{
    public class HttpRequestLogger : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.EndRequest += context_EndRequest;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            OutputFilterStream filter = (OutputFilterStream) HttpContext.Current.Items["filter"];
            System.Diagnostics.Debug.WriteLine("Response");
            System.Diagnostics.Debug.WriteLine(filter.ReadStream());
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpRequest request = HttpContext.Current.Request;
            System.Diagnostics.Debug.WriteLine("Request");
            System.Diagnostics.Debug.WriteLine(request.HttpMethod + " " + request.Url + " " +
                                               request.ServerVariables["SERVER_PROTOCOL"]);
            System.Diagnostics.Debug.WriteLine(new StreamReader(request.InputStream).ReadToEnd());
            request.InputStream.Position = 0;

            HttpResponse response = HttpContext.Current.Response;
            OutputFilterStream filter = new OutputFilterStream(response.Filter);
            response.Filter = filter;

            HttpContext.Current.Items.Add("filter", filter);
        }
    }
}
