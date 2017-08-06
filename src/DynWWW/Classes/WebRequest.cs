using Autodesk.DesignScript.Runtime;
using DynWWW.Helpers;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSCore.Web
{
    //[IsVisibleInDynamoLibrary(false)]
    public class WebRequest
    {
        #region constants

        /// <summary>
        /// The default timeout to wait for a request to return, expressed in miliseconds
        /// </summary>
        private const int defaultRequestTimeout = 1500;

        /// <summary>
        /// The default number of times to retry a failed request
        /// </summary>
        private const int defaultRequestAttempts = 3;

        /// <summary>
        /// The default security protocol to default to.
        /// The protocol is required for interaction with HTTPS endpoints, irrespective of using System.Net or RestSharp libraries.
        /// </summary>
        private const SecurityProtocolType defaultSecurityProtocol = SecurityProtocolType.Tls12;

        #endregion

        #region private properties

        /// <summary>
        /// The encapsulated Restsharp web request
        /// </summary>
        private RestRequest restRequest = new RestRequest();

        /// <summary>
        /// The encapsulated response from the server
        /// </summary>
        private WebResponse response = new WebResponse(new RestResponse());

        private System.TimeSpan timeToComplete;
        private Uri url;

        #endregion

        #region public properties

        /// <summary>
        /// The response the server returned on the last execution of the request.
        /// </summary>
        public WebResponse Respose => this.response;

        /// <summary>
        /// The URL for the request
        /// </summary>
        public string URL
        {
            get => url.ToString();
            set { url = WebHelpers.ParseUriFromString(value.ToString()); }
        }

        /// <summary>
        /// The time it took for the request to complete
        /// </summary>
        public System.TimeSpan Time { get => this.timeToComplete; }

        /// <summary>
        /// Set this property to true to force requests to go through a security protocol (TLS1.2).
        /// This is required when interacting with servers and APIs that require use of HTTPS as a protocol rather than HTTP.
        /// </summary>
        public bool ForceSecurityProtocol = false;

        #endregion

        #region constructor methods

        /// <summary>
        /// Build a simple GET web request to the specified URL
        /// </summary>
        /// <param name="url">The URL to send the request to.</param>
        /// <returns>The request object, ready for execution.</returns>
        public WebRequest(string url)
        {
            URL = url;
            restRequest = new RestRequest(this.URL, Method.GET);
            restRequest.Resource = "";
        }
        #endregion

        #region extension methods

        /// <summary>
        /// Sets the URL of the request.
        /// </summary>
        /// <param name="url">The URL to set for the request.</param>
        /// <returns>The request with an updated URL.</returns>
        public WebRequest SetUrl(string url)
        {
            this.URL = url;
            return this;
        }

        /// <summary>
        /// Adds a parameter to the web request
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to pass along.</param>
        /// <param name="type">The type of parameter.</param>
        /// <returns></returns>
        public WebRequest AddParameter(string name, object value, ParameterType type)
        {
            if (System.String.IsNullOrEmpty(name) || value == null)
            {
                throw new ArgumentNullException(DynWWW.Properties.Resources.WebRequestParameterNullMessage);
            }

            this.restRequest.AddParameter(name, value, type);

            return this;
        }

        #endregion

        #region Execution

        /// <summary>
        /// Executes a WebRequest and returns the response from the server.
        /// </summary>
        /// <param name="request">The web request to execute.</param>
        /// <returns>The response from the server as a WebResponse object.</returns>
        public static WebResponse Execute(WebRequest request)
        {
            // build a client to execute the request, recording start & end time
            var startTime = DateTime.Now;
            var client = new RestClient(request.URL);
            var responseFromServer = client.Execute(request.restRequest);
            var endTime = DateTime.Now;

            // if a network error occured, the request never reached the recipient server
            // in that case, expose the error in the UI through an Exception
            if (responseFromServer.ResponseStatus == ResponseStatus.Error)
            {
                throw new InvalidOperationException(DynWWW.Properties.Resources.WebRequestExecutionNetworkErrorMessage);
            }

            // update the request properties with response data
            request.response = new WebResponse(responseFromServer);
            request.timeToComplete = endTime - startTime;

            return request.response;
        }

        /// <summary>
        /// Deserialises a web response to the type of a supplied object.
        /// Accepts JSON and XML as valid response contents.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize to.</typeparam>
        /// <param name="response">The response from the server that needs to be deserialised.</param>
        /// <param name="obj">The object that will be used to determine what type to deserialise to.</param>
        /// <returns>The response deserialised as same type as supplied object.</returns>
        public T Deserialize<T>(WebResponse response, T obj) where T : new()
        {
            return DeserializeObject<T>(response);
        }

        /// <summary>
        /// Deserialises a web response the specified type.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize to.</typeparam>
        /// <param name="response">The response from the server that needs to be deserialised.</param>
        /// <returns>The deserialised object.</returns>
        private T DeserializeObject<T>(WebResponse response) where T : new()
        {
            var responseData = response.Content;

            /// We don't want the deserialisation to break if some properties are empty.
            /// So we need to specify the behaviour when such values are encountered.
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            return JsonConvert.DeserializeObject<T>(responseData, settings);
        }

        #endregion
    }
}
