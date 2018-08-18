using DynaWeb.Properties;
using RestSharp;
using System;
using System.Net;

namespace DynaWeb
{
    /// <summary>
    /// Provides support for executing WebRequests
    /// </summary>
    public static class Execute
    {
        #region internal methods
        private static WebResponse ClientRequestMethod(WebClient webClient, WebRequest webRequest)
        {
            if (webRequest == null) throw new ArgumentNullException(Resources.WebClientRequestNullMessage);
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

            // validate the Uri before attempting to execute the request
            try
            {
                var uri = WebClient.BuildUri(client, request);
                if (string.IsNullOrEmpty(uri) || !Helpers.IsUrlValid(Helpers.ParseUriFromString(uri)))
                {
                    throw new InvalidOperationException();
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    Resources.WebClientBuildUrlFailed +
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
            var responseFromServer = client.restClient.Execute(webRequest.restRequest);
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
            }

            // update the request properties with response data
            webRequest.response = new WebResponse(responseFromServer);
            webRequest.response.Time = endTime - startTime;

            return webRequest.response;
        }
        #endregion

        #region extension methods

        /// <summary>
        /// Execute the given request, on a client if one is supplied.
        /// </summary>
        /// <param name="webClient">The client that will execute the request.
        /// Note : the WebClient can be NULL when executing a WebRequest directly.</param>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="method">The string that represents the http method.
        /// Valid input : GET, DELETE, HEAD, OPTIONS, POST, PUT, MERGE.</param>
        /// <returns>The WebResponse from the server.</returns>
        public static WebResponse ByClientRequestMethod(WebClient webClient, WebRequest webRequest, string method = null)
        {
            // initialise method with GET as default 
            webRequest.restRequest.Method = Method.GET;
            // then try to parse value given (if any). If this fails, method is not changed.
            if (!string.IsNullOrEmpty(method) && Enum.TryParse<Method>(method, true, out Method reqMethod))
                webRequest.restRequest.Method = reqMethod;

            return ClientRequestMethod(webClient, webRequest);
        }

        /// <summary>
        /// Execute the given request, on a client if one is supplied, using the standard http GET method.
        /// </summary>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="webClient">(optional) The client that will execute the request.</param>
        /// <returns>The response from the server.</returns>
        public static WebResponse GET(WebRequest webRequest, WebClient webClient = null)
        {
            webRequest.restRequest.Method = Method.GET;
            return ClientRequestMethod(webClient, webRequest);
        }

        /// <summary>
        /// Execute the given request, on a client if one is supplied, using the standard http POST method.
        /// </summary>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="webClient">(optional) The client that will execute the request.</param>
        /// <returns>The response from the server.</returns>
        public static WebResponse POST(WebRequest webRequest, WebClient webClient = null)
        {
            webRequest.restRequest.Method = Method.POST;
            return ClientRequestMethod(webClient, webRequest);
        }

        /// <summary>
        /// Execute the given request, on a client if one is supplied, using the standard http PUT method.
        /// </summary>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="webClient">(optional) The client that will execute the request.</param>
        /// <returns>The response from the server.</returns>
        public static WebResponse PUT(WebRequest webRequest, WebClient webClient = null)
        {
            webRequest.restRequest.Method = Method.PUT;
            return ClientRequestMethod(webClient, webRequest);
        }

        /// <summary>
        /// Execute the given request, on a client if one is supplied, using the standard http DELETE method.
        /// </summary>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="webClient">(optional) The client that will execute the request.</param>
        /// <returns>The response from the server.</returns>
        public static WebResponse DELETE(WebRequest webRequest, WebClient webClient = null)
        {
            webRequest.restRequest.Method = Method.DELETE;
            return ClientRequestMethod(webClient, webRequest);
        }

        /// <summary>
        /// Execute the given request, on a client if one is supplied, using the standard http HEAD method.
        /// </summary>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="webClient">(optional) The client that will execute the request.</param>
        /// <returns>The response from the server.</returns>
        public static WebResponse HEAD(WebRequest webRequest, WebClient webClient = null)
        {
            webRequest.restRequest.Method = Method.HEAD;
            return ClientRequestMethod(webClient, webRequest);
        }

        /// <summary>
        /// Execute the given request, on a client if one is supplied, using the standard http MERGE method.
        /// </summary>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="webClient">(optional) The client that will execute the request.</param>
        /// <returns>The response from the server.</returns>
        public static WebResponse MERGE(WebRequest webRequest, WebClient webClient = null)
        {
            webRequest.restRequest.Method = Method.MERGE;
            return ClientRequestMethod(webClient, webRequest);
        }

        /// <summary>
        /// Execute the given request, on a client if one is supplied, using the standard http OPTIONS method.
        /// </summary>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="webClient">(optional) The client that will execute the request.</param>
        /// <returns>The response from the server.</returns>
        public static WebResponse OPTIONS(WebRequest webRequest, WebClient webClient = null)
        {
            webRequest.restRequest.Method = Method.OPTIONS;
            return ClientRequestMethod(webClient, webRequest);
        }

        /// <summary>
        /// Execute the given request, on a client if one is supplied, using the standard http PATCH method.
        /// </summary>
        /// <param name="webRequest">The request to be executed.</param>
        /// <param name="webClient">(optional) The client that will execute the request.</param>
        /// <returns>The response from the server.</returns>
        public static WebResponse PATCH(WebRequest webRequest, WebClient webClient = null)
        {
            webRequest.restRequest.Method = Method.PATCH;
            return ClientRequestMethod(webClient, webRequest);
        }

        #endregion
    }
}
