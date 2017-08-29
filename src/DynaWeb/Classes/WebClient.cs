using Autodesk.DesignScript.Runtime;
using RestSharp;
using System;

namespace DynaWeb
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

        #region public client settings

        /// <summary>
        /// This is the base URL for all future requests made by its client.
        /// This URL is combined with the request resource (ex: DynamoDS/Dynamo/issues) to construct the final URL for request.
        /// (ex: https://github.com/DynamoDS/Dynamo/issues)
        /// Should include scheme (ex: http://) and domain (ex: www.dynamobim.org) without trailing slash (/).
        /// </summary>
        public Uri BaseUrl { get => restClient.BaseUrl; set => restClient.BaseUrl = value; }

        /// <summary>
        /// Optional UserAgent to use for requests made by this client instance. (ex: Dynamo1.3)
        /// Used by the server to record where the request is coming from.
        /// </summary>
        public string UserAgent { get => restClient.UserAgent; set => restClient.UserAgent = value; }

        /// <summary>
        /// Timeout in milliseconds to use for requests made by this client instance
        /// </summary>
        public int Timeout { get => restClient.Timeout; set => restClient.Timeout = value; }

        /// <summary>
        /// Determine whether or not requests that result in HTTP status codes of 3xx should follow returned redirect. Defaults to true.
        /// </summary>
        public bool FollowRedirects
        {
            get => restClient.FollowRedirects;
            set
            {
                restClient.FollowRedirects = value;
                if (restClient.FollowRedirects == true && MaxRedirects == null)
                    MaxRedirects = 1;
            }
        }

        /// <summary>
        /// Maximum number of redirects to follow if FollowRedirects is true. Defaults to 1 if not set.
        /// </summary>
        public int? MaxRedirects
        {
            get
            {
                if (FollowRedirects == true) return restClient.MaxRedirects;
                else return null;
            }
            set => restClient.MaxRedirects = value;
        }

        /// <summary>
        /// Use this to override which node in the JSON response is used as the root/starting point for deserialisation.
        /// </summary>
        public string JsonTokenOverride { get; set; }

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
        public static WebClient ByUrl(string baseUrl)
        {
            return new WebClient(baseUrl, "");
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
        public static WebClient ByUrlToken(string baseUrl, string token)
        {

            return new WebClient(baseUrl, token);
        }

        private WebClient(string baseUrl, string token)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientUrlNullMessage);
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientTokenNullMessage);

            this.restClient = new RestClient(baseUrl);
            this.authToken = token;
            this.UserAgent = "DynamoDS";
        }

        #endregion

        #region execution

        /// <summary>
        /// Executes a WebRequest in the context of the client and returns the response from the server.
        /// </summary>
        /// <param name="client">The WebClient to use for execution of request.</param>
        /// <param name="request">The web WebRequest to execute.</param>
        /// <returns>The response from the server as a WebResponse object.</returns>
        [CanUpdatePeriodically(true)]
        public static WebResponse Execute(WebClient client, WebRequest request)
        {
            if (client==null) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientNullMessage);
            if (request == null) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebRequestNullMessage);

            request.response = DynaWeb.Execute.ByClientRequestMethod(client, request);
            return request.response;
        }

        #endregion

        #region public static methods

        /// <summary>
        /// Assembles the URL to call based on parameters, method and resource.
        /// Not needed to run the request, but useful for debugging purposes.
        /// </summary>
        /// <param name="client">The WebClient to update.</param>
        /// <param name="request">The request to execute</param>
        /// <returns>A string representation of the assembly Uri</returns>
        public static string BuildUri(WebClient client, WebRequest request)
        {
            if (request == null) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientRequestNullMessage);

            return client.restClient.BuildUri(request.restRequest).ToString();
        }

        /// <summary>
        /// Set the base URL for this client.
        /// </summary>
        /// <param name="client">The WebClient to update.</param>
        /// <param name="url">The value to set BaseUrl to, has to be a valid URL.</param>
        /// <returns>The WebClient supplied with an updated BaseUrl property.</returns>
        public static WebClient SetBaseURL(WebClient client, string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientUrlNullMessage);
            if (!Helpers.CheckURI(Helpers.ParseUriFromString(url))) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebUrlInvalidMessage);
            client.BaseUrl = Helpers.ParseUriFromString(url);
            return client;
        }

        /// <summary>
        /// Set the user agent communicated with requests this client sends.
        /// </summary>
        /// <param name="client">The WebClient to update.</param>
        /// <param name="userAgent">The value to set the UserAgent to.</param>
        /// <returns>The WebClient supplied with the an UserAgent property.</returns>
        public static WebClient SetUserAgent(WebClient client, string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientUserAgentNullMessage);
            client.UserAgent = userAgent;
            return client;
        }

        /// <summary>
        /// Set the timeout in milliseconds to use for requests made by this client instance
        /// </summary>
        /// <param name="client">The WebClient to update.</param>
        /// <param name="timeout">The value to set timeout to, expressed in milliseconds.</param>
        /// <returns>The WebClient supplied with an updated Timeout property.</returns>
        public static WebClient SetTimeout(WebClient client, int timeout)
        {
            if (timeout <= 0) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientTimeoutInvalidMessage);
            client.Timeout = timeout;
            return client;
        }

        /// <summary>
        /// Sets the FollowRedirects setting of the client. This controls whether or not requests that result in HTTP status codes of 3xx should follow returned redirect. Default is true.
        /// </summary>
        /// <param name="client">The WebClient to update.</param>
        /// <param name="followRedirects">True to follow redirects, false to end request.</param>
        /// <returns>The WebClient supplied with an updated FollowRedirects property.</returns>
        public static WebClient SetFollowRedirects(WebClient client, bool followRedirects = true)
        {
            client.FollowRedirects = followRedirects;
            return client;
        }

        /// <summary>
        /// Set the maximum number of redirects to follow if FollowRedirects is true.
        /// </summary>
        /// <param name="client">The WebClient to update.</param>
        /// <param name="maxRedirects">The value to set maximum to, expressed as an integer.</param>
        /// <returns>The WebClient supplied with an updated MaxRedirects property.</returns>
        public static WebClient SetMaxRedirects(WebClient client, int maxRedirects)
        {
            if (maxRedirects <= 0) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientTimeoutInvalidMessage);
            client.MaxRedirects = maxRedirects;
            return client;
        }

        /// <summary>
        /// Set the JsonTokenOverride that is used for deserialisation purposes.
        /// </summary>
        /// <param name="client">The WebClient to update.</param>
        /// <param name="jsonToken">The value to set JsonTokenOverride to.</param>
        /// <returns>The WebClient supplied with an updated JsonTokenOverride property.</returns>
        public static WebClient SetJsonTokenOverride(WebClient client, string jsonToken)
        {
            if (string.IsNullOrEmpty(jsonToken)) throw new ArgumentNullException(DynaWeb.Properties.Resources.WebClientTokenNullMessage);
            client.JsonTokenOverride = jsonToken;
            return client;
        }

        #endregion
    }
}
