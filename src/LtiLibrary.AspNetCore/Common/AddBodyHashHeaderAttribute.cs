using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

#if NETSTANDARD2_0 || NETCOREAPP3_1
using Microsoft.AspNetCore.Http;
#else
using Microsoft.AspNetCore.Http.Internal;
#endif

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
    internal class AddBodyHashHeaderAttribute : Attribute, IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;

            // Re-reading ASP.Net Core request bodies with EnableBuffering()
            // https://devblogs.microsoft.com/aspnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/

            // HTTP: Synchronous IO disabled in all servers
            // https://docs.microsoft.com/en-us/dotnet/core/compatibility/2.2-3.0#http-synchronous-io-disabled-in-all-servers

#if NETSTANDARD2_0 || NETCOREAPP3_1
            request.EnableBuffering();
#else
            request.EnableRewind();
#endif

            if (request.Body.CanRead)
            {
                // Calculate the body hash
                try
                {
                    // Leave the body open so the next middleware can read it.
                    using (var reader = new System.IO.StreamReader(
                        request.Body, 
                        encoding: System.Text.Encoding.UTF8, 
                        detectEncodingFromByteOrderMarks: false, 
                        bufferSize: 1024 /* default */, 
                        leaveOpen: true))
                    {
                        var body = await reader.ReadToEndAsync();

                        using (var sha1 = SHA1.Create())
                        {
                            // Add HashAlgorithm.ComputeHashAsync (.NET Core 5.0)
                            // https://github.com/dotnet/corefx/pull/42565

                            var hash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(body));
                            var hash64 = Convert.ToBase64String(hash);
                            request.Headers.Add("BodyHash", hash64);
                        }
                    }
                }
                finally
                {
                    if (request.Body.CanSeek)
                    {
                        request.Body.Position = 0;
                    }
                }
            }

            await next().ConfigureAwait(false);
        }
    }
}
