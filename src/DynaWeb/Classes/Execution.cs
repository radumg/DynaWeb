using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DynaWeb
{
    /// <summary>
    /// Provides support for executing WebRequests
    /// </summary>
    public static class Execution
    {
        #region standard method
        /// <summary>
        /// Execute the given request, on a client if one is supplied.
        /// </summary>
        /// <param name="webClient">The client that will execute the request.
        /// Note : the WebClient can be NULL when executing a WebRequest directly.</param>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="method"></param>
        /// <returns>The WebResponse from the server.</returns>
        public static WebResponse ByClientRequestMethod(WebClient webClient, WebRequest webRequest, string method = null)
        {
            if (webRequest == null) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientRequestNullMessage);
            // build a client & request to execute
            WebClient client;
            WebRequest request = webRequest;

            // check if client is null : this will be the case when executing a WebRequest directly
            if (webClient == null)
            {
                // in that case, an empty WebClient will be constructed with the WebRequest URL as its baseUrl.
                // the request Resource also needs to be reset, otherwise the URL would be concatenating to itself.
                client = WebClient.ByUrl(webRequest.URL);
                request.Resource = "";
            }
            else
            {
                client = webClient;
            }
            client.UserAgent = "DynamoDS";

            // initialise method with GET as default then try to parse value given (if any).
            request.restRequest.Method = Method.GET;
            if (!string.IsNullOrEmpty(method) && Enum.TryParse<Method>(method, true, out Method reqMethod))
                request.restRequest.Method = reqMethod;

            // validate the Uri before attempting to execute the request
            try
            {
                var uri = WebClient.BuildUri(client, request);
                if (string.IsNullOrEmpty(uri) || DynaWeb.Helpers.CheckURI(Helpers.ParseUriFromString(uri)) != true)
                {
                    //TODO : error handling here is limited, needs checking and expanding.  
                    throw new InvalidOperationException("Malformed URL.");
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    DynaWeb.Properties.Resources.WebClientBuildUrlFailed +
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
                    throw new InvalidOperationException(DynaWeb.Properties.Resources.WebResponseNetworkErrorMessage);
                case ResponseStatus.Error:
                    throw new InvalidOperationException(DynaWeb.Properties.Resources.WebResponseNetworkErrorMessage);
                case ResponseStatus.TimedOut:
                    throw new InvalidOperationException(DynaWeb.Properties.Resources.WebRequestTimedOutMessage);
                case ResponseStatus.Aborted:
                    throw new InvalidOperationException(DynaWeb.Properties.Resources.WebResponseAbortedMessage);
                default:
                    break;
            }

            // update the request properties with response data
            webRequest.response = new WebResponse(responseFromServer);
            webRequest.response.Time = endTime - startTime;

            return webRequest.response;
        }
        #endregion

        #region extension methods

        public static WebResponse GET()
        {
            return null;
        }

        #endregion
    }
}
