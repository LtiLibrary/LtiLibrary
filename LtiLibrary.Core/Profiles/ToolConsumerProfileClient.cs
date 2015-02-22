using System;
using System.Net;
using System.Threading.Tasks;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Extensions;

namespace LtiLibrary.Core.Profiles
{
    public static class ToolConsumerProfileClient
    {
        public static async Task<ToolConsumerProfileResponse> GetToolConsumerProfile(string serviceUrl)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
                request.Method = "GET";
                request.Accept = LtiConstants.ToolConsumerProfileMediaType;
                request.AllowAutoRedirect = true;

                return await Task.Factory.StartNew(() =>
                {
                    var profileResponse = new ToolConsumerProfileResponse();
                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        profileResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            profileResponse.ToolConsumerProfile = response.DeserializeObject<ToolConsumerProfile>();
                        }
                    }
                    catch (WebException ex)
                    {
                        response = (HttpWebResponse)ex.Response;
                        profileResponse.StatusCode = response.StatusCode;
                    }
                    catch (Exception)
                    {
                        profileResponse.StatusCode = HttpStatusCode.InternalServerError;
                    }
                    finally
                    {
#if DEBUG
                        profileResponse.HttpRequest = request.ToFormattedRequestString();
                        if (response != null)
                        {
                            profileResponse.HttpResponse = response.ToFormattedResponseString(
                                profileResponse.ToolConsumerProfile == null
                                ? null
                                : profileResponse.ToolConsumerProfile.ToJsonLdString());
                        }
#endif
                    }
                    return profileResponse;
                });
            }
            catch (Exception)
            {
                return new ToolConsumerProfileResponse { StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}
