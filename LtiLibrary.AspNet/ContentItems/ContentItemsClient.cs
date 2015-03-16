using System;
using LtiLibrary.AspNet.Lti1;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.ContentItems;
using LtiLibrary.Core.Extensions;
using LtiLibrary.Core.Lti1;
using LtiRequestViewModel = LtiLibrary.AspNet.Lti1.LtiRequestViewModel;

namespace LtiLibrary.AspNet.ContentItems
{
    public static class ContentItemsClient
    {
        /// <summary>
        /// Create an LtiRequestViewModel that contains a ContentItemPlacementResponse.
        /// </summary>
        /// <param name="url">The content_item_return_url from the content-item message.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to sign the request.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to sign the request.</param>
        /// <param name="contentItems">The ContentItemPlacementResponse to send.</param>
        /// <param name="data">The data received in the original content-item message.</param>
        /// <returns>The LtiRequestViewModel which contains a signed version of the response.</returns>
        public static LtiRequestViewModel CreateContentItemSelectionViewModel(
            string url, string consumerKey, string consumerSecret,
            ContentItemSelectionGraph contentItems, string data)
        {
            return CreateContentItemSelectionViewModel(url, consumerKey, consumerSecret, contentItems, data, null, null, null, null);
        }

        /// <summary>
        /// Create an LtiRequestViewModel that contains a ContentItemPlacementResponse.
        /// </summary>
        /// <param name="url">The content_item_return_url from the content-item message.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to sign the request.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to sign the request.</param>
        /// <param name="contentItems">The ContentItemPlacementResponse to send.</param>
        /// <param name="data">The data received in the original content-item message.</param>
        /// <param name="ltiErrorLog">Optional plain text error message to be logged by the Tool Consumer.</param>
        /// <param name="ltiErrorMsg">Optional plain text error message to be displayed by the Tool Consumer.</param>
        /// <param name="ltiLog">Optional plain text message to be logged by the Tool Consumer.</param>
        /// <param name="ltiMsg">Optional plain text message to be displayed by the Tool Consumer.</param>
        /// <returns>The LtiRequestViewModel which contains a signed version of the response.</returns>
        public static LtiRequestViewModel CreateContentItemSelectionViewModel(
            string url, string consumerKey, string consumerSecret,
            ContentItemSelectionGraph contentItems, string data,
            string ltiErrorLog, string ltiErrorMsg, string ltiLog, string ltiMsg)
        {
            var ltiRequest = new LtiRequest(LtiConstants.ContentItemSelectionLtiMessageType)
            {
                Url = new Uri(url),
                ConsumerKey = consumerKey,
                ContentItems = contentItems.ToJsonLdString(),
                Data = data,
                LtiErrorLog = ltiErrorLog,
                LtiErrorMsg = ltiErrorMsg,
                LtiLog = ltiLog,
                LtiMsg = ltiMsg
            };

            return ltiRequest.GetViewModel(consumerSecret);
        }
    }
}
