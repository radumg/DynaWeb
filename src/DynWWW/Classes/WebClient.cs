using Autodesk.DesignScript.Runtime;
using RestSharp;
using System;
using System.Net;

namespace DSCore.Web
{
    /// <summary>
    /// A web client is used to translate request objects into HTTP requests and process the server response.
    /// The web client also represents a uniquely configured connection to a server or service.
    /// </summary>
    public class WebClient
    {

        #region private members
        /// <summary>
        /// This is the wrapped RestSharp RestClient.
        /// </summary>
        internal RestClient restClient;

        /// <summary>
        /// This auth token is used to authenticate requests made by the client.
        /// Use it as the private store for OAuth tokens for example.
        /// </summary>
        private string authToken;

        /// <summary>
        /// This auth code is used during OAuth authentication flows.
        /// </summary>
        private string authCode;

        /// <summary>
        /// This string is used to verify responses from the server during authentication flows.
        /// </summary>
        private string authVerification;
        #endregion

        #region public members
        /// <summary>
        /// This is the base URL for all future requests made by its client.
        /// This URL is combined with the request resource (ex: DynamoDS/Dynamo/issues) to construct the final URL for request.
        /// (ex: https://github.com/DynamoDS/Dynamo/issues)
        /// Should include scheme (ex: http://) and domain (ex: www.dynamobim.org) without trailing slash (/).
        /// </summary>
        public Uri BaseUrl { get => restClient.BaseUrl; set => restClient.BaseUrl = value; }

        /// <summary>
        /// Use this to override which node in the JSON response is used as the root/starting point for deserialisation.
        /// </summary>
        public string JsonTokenOverride { get; set; }
        #endregion

        #region client settings

        /// <summary>
        /// Optional UserAgent to use for requests made by this client instance. (ex: Dynamo1.3)
        /// Used by the server to record where the request is coming from.
        /// </summary>
        public string UserAgent { get => restClient.UserAgent; set => restClient.UserAgent = value; }

        /// <summary>
        /// Specify the timeout in milliseconds to use for requests made by this client instance
        /// </summary>
        public int Timeout { get => restClient.Timeout; set => restClient.Timeout = value; }
        #endregion


        #region constructors

        /// <summary>
        /// Build a new WebClient using the specified URL as its base.
        /// A web client is used to translate request objects into HTTP requests and process the server response.
        /// The web client also represents a uniquely configured connection to a server or service.
        /// </summary>
        /// <param name="baseUrl">The URL to use for all future requests made by this client.
        /// Should include scheme (ex: http://) and domain (ex: www.dynamobim.org) without trailing slash (/).
        /// </param>
        public WebClient(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientUrlNullMessage);

            Initialize(baseUrl, "");
        }

        /// <summary>
        /// Build a new WebClient using the specified URL as its base.
        /// A web client is used to translate request objects into HTTP requests and process the server response.
        /// The web client also represents a uniquely configured connection to a server or service.
        /// </summary>
        /// <param name="baseUrl">The URL to use for all future requests made by this client.
        /// Should include scheme (ex: http://) and domain (ex: www.dynamobim.org) without trailing slash (/).
        /// </param>
        /// <param name="token">The auth token is used to authenticate requests made by the client.
        /// Use it as the private store for OAuth tokens for example.
        /// Once the client is created, this cannot be changed.</param>
        public WebClient(string baseUrl, string token)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientUrlNullMessage);
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientTokenNullMessage);

            Initialize(baseUrl, token);
        }

        private void Initialize(string baseUrl, string token)
        {
            this.restClient = new RestClient(baseUrl);
            this.authToken = token;
            this.UserAgent = "DynamoDS";
        }

        #endregion

        #region methods

        /// <summary>
        /// Executes a WebRequest in the context of the client and returns the response from the server.
        /// </summary>
        /// <param name="request">The web request to execute.</param>
        /// <returns>The response from the server as a WebResponse object.</returns>
        [CanUpdatePeriodically(true)]
        public DSCore.Web.WebResponse Execute(DSCore.Web.WebRequest request)
        {
            // build a client to execute the request, recording start & end time
            var startTime = DateTime.Now;
            

            var responseFromServer = client.Execute(request.GetInternalRequest());
            var endTime = DateTime.Now;

            if (request.ForceSecurityProtocol)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit *= 10;
            }

            // if a network error occured, the request never reached the recipient server
            // in that case, expose the error in the UI through an Exception
            if (responseFromServer.ResponseStatus == ResponseStatus.Error)
            {
                throw new InvalidOperationException(DynWWW.Properties.Resources.WebRequestExecutionNetworkErrorMessage);
            }

            // update the request properties with response data
            request.response = new DSCore.Web.WebResponse(responseFromServer);
            request.timeToComplete = endTime - startTime;

            return request.response;
        }



        /// <summary>
        /// Assembles the URL to call based on parameters, method and resource.
        /// Not needed to run the request, but useful for debugging purposes.
        /// </summary>
        /// <param name="request">The request to execute</param>
        /// <returns>A string representation of the assembly Uri</returns>
        public string BuildUri(WebRequest request) {
            if (request == null) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientRequestNullMessage);

            return this.restClient.BuildUri(request.GetInternalRequest()).ToString();
        }

        #endregion
    }
}
