using System;
using System.Collections.Specialized;
using LtiLibrary.Core.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace LtiLibrary.AspNet.Extensions
{
    public static class OAuthRequestExtensions
    {
        public static void ParseRequest(this IOAuthRequest oauthRequest, HttpRequest request)
        {
            oauthRequest.HttpMethod = request.Method;
            oauthRequest.Url = new Uri(request.GetDisplayUrl());

            // Launch requests pass parameters as form fields
            var form = new NameValueCollection();
            foreach (var formKey in request.Form.Keys)
            {
                form.Set(formKey, request.Form[formKey]);
            }
            oauthRequest.Parameters.Add(form);
        }
    }
}
