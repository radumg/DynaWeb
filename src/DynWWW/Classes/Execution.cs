using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSCore.Web
{
    /// <summary>
    /// Provides support for executing WebRequests
    /// </summary>
    internal static class Execution
    {
        /// <summary>
        /// Execute the given request, on a client if one is supplied.
        /// </summary>
        /// <param name="webClient">The client that will execute the request.
        /// Note : the WebClient can be NULL when executing a WebRequest directly.</param>
        /// <param name="webRequest">The request to be executed.</param>
        /// <returns>The WebResponse from the server.</returns>
        internal static WebResponse ByClientRequest(WebClient webClient, WebRequest webRequest)
        {
            if (webRequest == null) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientRequestNullMessage);
            // build a client & request to execute
            WebClient client;
            WebRequest request = webRequest;

            // check if client is null : this will be the case when executing a WebRequest directly
            if (webClient == null)
            {
                // in that case, an empty WebClient will be constructed with the WebRequest URL as its baseUrl.
                // the request Resource also needs to be reset, otherwise the URL would be concatenating to itself.
                client = new WebClient(webRequest.URL);
                request.Resource = "";
            }
            else
            {
                client = webClient;
            }
            client.UserAgent = "DynamoDS";

            // validate the Uri before attempting to execute the request
            try
            {
                var uri = client.BuildUri(request);
                if (string.IsNullOrEmpty(uri) || DSCore.Web.Helpers.CheckURI(Helpers.ParseUriFromString(uri)) != true)
                {
                    //TODO : error handling here is limited, needs checking and expanding.  
                    throw new InvalidOperationException("Malformed URL.");
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    DynWWW.Properties.Resources.WebClientBuildUrlFailed +
                    Environment.NewLine +
                    "Error returned was :" + Environment.NewLine +
                    e.Message);
            }

            // enforce security protocols if needed
            if (webRequest.ForceSecurityProtocol)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit *= 10;
            }

            // Execute using the wrapped client and wrapped request objects.
            var startTime = DateTime.Now;
            var responseFromServer = client.restClient.Execute(webRequest.GetInternalRequest());
            var endTime = DateTime.Now;

            // the server response needs to be handled based on status and any errors raised in UI
            switch (responseFromServer.ResponseStatus)
            {
                case ResponseStatus.None:
                    throw new InvalidOperationException(DynWWW.Properties.Resources.WebResponseNetworkErrorMessage);
                    break;
                case ResponseStatus.Completed:
                    break;
                case ResponseStatus.Error:
                    throw new InvalidOperationException(DynWWW.Properties.Resources.WebResponseNetworkErrorMessage);
                    break;
                case ResponseStatus.TimedOut:
                    throw new InvalidOperationException(DynWWW.Properties.Resources.WebResponseTimedOutMessage);
                    break;
                case ResponseStatus.Aborted:
                    throw new InvalidOperationException(DynWWW.Properties.Resources.WebResponseAbortedMessage);
                    break;
                default:
                    break;
            }

            // update the request properties with response data
            webRequest.response = new WebResponse(responseFromServer);
            webRequest.timeToComplete = endTime - startTime;

            return webRequest.response;
        }
    }
}
