using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LtiLibrary.AspNetCore.Common
{
    /// <summary>
    /// Calculate the incomming body hash and store it as a header in the request.
    /// </summary>
    /// <remarks>
    /// The Request.Body stream is disposed during ModelBinding, making it impossible
    /// to calculate the incomming body hash inside the Action. This ResourceFilter
    /// is executed just before ModelBinding and the hash is stored in a header that
    /// the Action can access.
    /// </remarks>
    public class AddBodyHashHeaderAttribute : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;

            if (request.Body.CanRead)
            {
                byte[] content;
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        await request.Body.CopyToAsync(ms);
                        content = ms.ToArray();
                    }
                    finally
                    {
                        if (request.Body.CanSeek)
                        {
                            request.Body.Position = 0;
                        }
                    }
                }

                // Calculate the body hash
                using (var sha1 = SHA1.Create())
                {
                    var hash = sha1.ComputeHash(content);
                    var hash64 = Convert.ToBase64String(hash);
                    request.Headers.Add("BodyHash", hash64);
                }
            }

            await next();
        }
    }
}
