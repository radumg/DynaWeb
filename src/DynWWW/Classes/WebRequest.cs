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
using RestSharp.Serializers;
using System.Reflection;

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

        #region IRestRequest interface fields

        /// <summary>
        /// Container of all HTTP parameters to be passed with the request.
        /// See <see cref="AddParameter(Parameter)"/> for explanation of the types of parameters that can be passed
        /// </summary>
        public List<Parameter> Parameters => restRequest.Parameters;

        /// <summary>
        /// Container of all the files to be uploaded with the request.
        /// </summary>
        public List<FileParameter> Files => restRequest.Files;

        /// <summary>
        /// Determines what HTTP method to use for this request.
        /// Supported methods: GET, POST, PUT, DELETE, HEAD, OPTIONS
        /// Default is GET
        /// </summary>
        public Method Method
        {
            get => restRequest.Method;
            set => restRequest.Method = value;
        }

        /// <summary>
        /// The Resource URL to make the request against, should not include the scheme or domain.
        /// Do not include leading slash. Combined with web client BaseUrl to assemble final URL:
        /// {BaseUrl}/{Resource} (BaseUrl is scheme + domain, e.g. http://example.com)
        /// </summary>
        public string Resource
        {
            get => restRequest.Resource;
            set => restRequest.Resource = value;
        }

        /// <summary>
        /// Used by the default deserializers to determine where to start deserializing from.
        /// Can be used to skip container or root elements that do not have corresponding deserialzation targets.
        /// Example : most APIs return values wrapped in a root "data" element.
        /// </summary>
        public string RootElement
        {
            get => restRequest.RootElement;
            set => restRequest.RootElement = value;
        }

        /// <summary>
        /// Timeout in milliseconds to be used for the request.
        /// This timeout value overrides a timeout set on the  Web Client.
        /// </summary>
        public int Timeout
        {
            get => restRequest.Timeout;
            set => restRequest.Timeout = value;
        }

        /// <summary>
        /// The number of milliseconds before the writing or reading times out.
        /// This timeout value overrides a timeout set on a Web Client.
        /// </summary>
        public int ReadWriteTimeout
        {
            get => restRequest.ReadWriteTimeout;
            set => restRequest.ReadWriteTimeout = value;
        }

        /// <summary>
        /// How many attempts were made to send this Request?
        /// This Number is incremented each time the web client sends the request.
        /// Useful when using Asynchronous Execution with Callbacks
        /// </summary>
        public int Attempts => restRequest.Attempts;

        /// <summary>
        /// Serializer to use. Can be JSON or XML, defaults to XML.
        /// </summary>
        public DataFormat RequestFormat
        {
            get => restRequest.RequestFormat;
            set => restRequest.RequestFormat = value;
        }

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
        /// Sets the HTTP method to use for the request.
        /// Valid input : GET, DELETE, HEAD, OPTIONS, POST, PUT, MERGE
        /// Note : input is not case-sensitive.
        /// </summary>
        /// <param name="method">The string that represents the http method.</param>
        /// <returns>The WebRequest updated with set method if input was valid, the unchanged WebRequest otherwise.</returns>
        public WebRequest SetMethod(string method)
        {
            if (Enum.TryParse<Method>(method, true, out Method reqMethod))
                this.restRequest.Method = reqMethod;
            return this;
        }

        /// <summary>
        /// Sets the default serialiser to use with this request.
        /// </summary>
        /// <param name="format">The serialiser to use as case-insensitive string.
        /// Valid inputs : JSON, XML</param>
        /// <returns>The updated request if supplied format was valid, the unchanged request if not.</returns>
        public WebRequest SetRequestFormat(string format)
        {
            if (Enum.TryParse<DataFormat>(format, true, out DataFormat dataFormat))
                this.restRequest.RequestFormat = dataFormat;
            return this;
        }

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
        /// Sets the value of the ForceSecurityProtocol property.
        /// Use this property to foce use of TLS1.2, required when interacting over HTTPS.
        /// </summary>
        /// <param name="forceSecurity">True or False</param>
        /// <returns>The request</returns>
        public WebRequest SetSecurityProtocol(bool forceSecurity)
        {
            this.ForceSecurityProtocol = forceSecurity;
            return this;
        }

        /// <summary>
        /// Adds a file to the Files collection to be included with a POST or PUT request (other methods do not support file uploads).
        /// </summary>
        /// <param name="name">The parameter name to use in the request</param>
        /// <param name="path">Full path to file to upload</param>
        /// <param name="contentType">The MIME type of the file to upload</param>
        /// <returns>This request</returns>
        public WebRequest AddFile(string name, string path, string contentType = null)
        {
            if (this.restRequest.Method != Method.POST || this.restRequest.Method != Method.PUT)
                throw new InvalidOperationException("Can only add a file to a POST or PUT request.");

            restRequest.AddFile(name, path, contentType);
            return this;
        }

        /// <summary>
        /// Serializes obj to data format specified by RequestFormat and adds it to the request body.
        /// The default format is XML. Change RequestFormat if you wish to use a different serialization format.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>This request</returns>
        public WebRequest AddBody(object obj)
        {
            if (this.restRequest.Method == Method.GET) throw new InvalidOperationException("Cannot add a body parameter to a GET request");

            restRequest.AddBody(obj);
            return this;
        }

        /// <summary>
        /// Serializes obj to JSON format and adds it to the request body.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>This request</returns>
        public WebRequest AddJsonBody(object obj)
        {
            if (this.restRequest.Method == Method.GET) throw new InvalidOperationException("Cannot add a body parameter to a GET request");

            restRequest.AddJsonBody(obj);
            return this;
        }

        /// <summary>
        /// Serializes obj to XML format and adds it to the request body.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>This request</returns>
        public WebRequest AddXmlBody(object obj)
        {
            if (this.restRequest.Method == Method.GET) throw new InvalidOperationException("Cannot add a body parameter to a GET request");

            restRequest.AddXmlBody(obj);
            return this;
        }

        /// <summary>
        /// Calls <see cref="AddParameter(string, object, ParameterType)"/>AddParameter() for all public, readable properties of obj
        /// </summary>
        /// <param name="obj">The object with properties to add as parameters</param>
        /// <returns>This request</returns>
        public WebRequest AddObject(object obj)
        {
            restRequest.AddObject(obj);
            return this;
        }

        /// <summary>
        /// Adds a HTTP parameter to the request.
        /// Uses QueryString for GET, DELETE, OPTIONS and HEAD, Encoded form for POST and PUT
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        /// <param name="parameterType">The type of the parameter to add.
        /// Valid inputs: Cookie, GetOrPost, HttpHeader, QueryString, RequestBody, UrlSegment</param>
        /// <returns>The request with the added parameter.</returns>
        public WebRequest AddParameter(string name, object value, string parameterType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name parameter cannot be null.");
            if (value.Equals(null))
                throw new ArgumentNullException("Value parameter cannot be null.");
            if (Enum.TryParse<ParameterType>(parameterType, true, out ParameterType pType) == false)
                throw new ArgumentException("Could not parse the supplied value into a valid Parameter Type.");

            restRequest.AddParameter(name, value, pType);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, HttpHeader)
        /// </summary>
        /// <param name="name">Name of the header to add</param>
        /// <param name="value">Value of the header to add</param>
        /// <returns></returns>
        public WebRequest AddHeader(string name, string value)
        {
            restRequest.AddHeader(name, value);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, Cookie)
        /// </summary>
        /// <param name="name">Name of the cookie to add</param>
        /// <param name="value">Value of the cookie to add</param>
        /// <returns></returns>
        public WebRequest AddCookie(string name, string value)
        {
            restRequest.AddCookie(name, value);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, UrlSegment)
        /// </summary>
        /// <param name="name">Name of the segment to add</param>
        /// <param name="value">Value of the segment to add</param>
        /// <returns></returns>
        public WebRequest AddUrlSegment(string name, string value)
        {
            restRequest.AddUrlSegment(name, value);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, QueryString)
        /// </summary>
        /// <param name="name">Name of the parameter to add</param>
        /// <param name="value">Value of the parameter to add</param>
        /// <returns></returns>
        public WebRequest AddQueryParameter(string name, string value)
        {
            restRequest.AddQueryParameter(name, value);
            return this;
        }

        #endregion

        #region Execution

        /// <summary>
        /// Executes a WebRequest and returns the response from the server.
        /// </summary>
        /// <param name="request">The web request to execute.</param>
        /// <returns>The response from the server as a WebResponse object.</returns>
        [CanUpdatePeriodically(true)]
        public static WebResponse Execute(WebRequest request)
        {
            // build a client to execute the request, recording start & end time
            var startTime = DateTime.Now;
            var client = new RestClient(request.URL);
            client.UserAgent = "DynamoDS";

            var responseFromServer = client.Execute(request.restRequest);
            var endTime = DateTime.Now;

            if (request.ForceSecurityProtocol)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit *= 10;
            }

            /// if a network error occured, the request never reached the recipient server
            /// in that case, expose the error in the UI through an Exception
            if (responseFromServer.ResponseStatus == ResponseStatus.Error)
            {
                throw new InvalidOperationException(DynWWW.Properties.Resources.WebRequestExecutionNetworkErrorMessage);
            }

            // update the request properties with response data
            request.response = new WebResponse(responseFromServer);
            request.timeToComplete = endTime - startTime;

            return request.response;
        }

        #endregion


    }
}
