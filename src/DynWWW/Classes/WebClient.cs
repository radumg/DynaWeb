using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynWWW.Classes
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
        private RestClient restClient;

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
        public string UserAgent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Specify the timeout in milliseconds to use for requests made by this client instance
        /// </summary>
        public int Timeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion


        #region constructor

        public WebClient(string baseUrl):this(baseUrl,"")
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientUrlNullMessage);
        }

        public WebClient(string baseUrl, string token)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientUrlNullMessage);
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientTokenNullMessage);

            restClient = new RestClient(baseUrl);
            this.authToken = token;

        }

        #endregion

        #region methods

        #endregion
    }
}
