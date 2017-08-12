using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DSCore.Web
{
    /// <summary>
    /// The response returned from a server after a WebRequest was executed
    /// </summary>
    public class WebResponse
    {
        #region private properties
        private IRestResponse response;
        #endregion

        #region public properties

        // properties relating to the content of the response
        public string Content => this.response.Content;
        public string ContentType => this.response.ContentType;
        public long ContentLength => this.response.ContentLength;
        public string ContentEncoding => this.response.ContentEncoding;
        public byte[] RawBytes => this.response.RawBytes;

        // properties relating to the status of the response
        public string StatusCode => this.response.StatusCode.ToString();
        public string StatusDescription => this.response.StatusDescription;

        // meta properties that have information about the response itself
        public Uri ResponseUri => this.response.ResponseUri;
        public string Server => this.response.Server;
        public List<List<string>> Headers
        {
            get
            {
                var headersDict = this.response.Headers.ToDictionary(x => x.Name);
                var headers = new List<List<string>>();
                headers.Add(headersDict.Keys.ToList());
                headers.Add(headersDict.Values.Select(x => x.Value.ToString()).ToList());

                return headers;
            }
        }
        public string Cookies => this.response.Cookies.ToString();
        public string ResponseStatus => this.response.ResponseStatus.ToString();
        public string ErrorException => this.response.ErrorException.ToString();
        public string ErrorMessage => this.response.ErrorMessage;
        #endregion

        #region constructor
        public WebResponse(IRestResponse res)
        {
            this.response = res;
        }
        #endregion

        #region deserialisation

        /// <summary>
        /// Deserialises a web response to the type of a supplied object.
        /// Accepts JSON and XML as valid response contents.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize to.</typeparam>
        /// <param name="response">The response from the server that needs to be deserialised.</param>
        /// <param name="obj">The object that will be used to determine what type to deserialise to.</param>
        /// <returns>The response deserialised as same type as supplied object.</returns>
        [CanUpdatePeriodically(true)]
        public static dynamic DeserializeAsObject(WebResponse response, object obj) 
        {
            var responseData = response.Content;

            /// We don't want the deserialisation to break if some properties are empty.
            /// So we need to specify the behaviour when such values are encountered.
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            var type = obj.GetType();

            return JsonConvert.DeserializeObject(responseData, type, settings);
        }

        /// <summary>
        /// Deserialises the JSON content of a WebResponse into a dictionary of string keys and object values.
        /// </summary>
        /// <param name="response">The response to deserialise</param>
        /// <returns>A dictionatry<string,object> of the responses's JSON content.</string></returns>
        [MultiReturn(new[] { "properties", "values" })]
        public static Dictionary<string, object> DeserialiseJsonToDictionary(WebResponse response, string jsonRoot = null)
        {
            var responseData = response.Content;

            string jsonData = "";
            if (string.IsNullOrEmpty(jsonRoot) == false)
            {
                JObject o = JObject.Parse(response.Content);
                jsonData = o.SelectToken(jsonRoot).ToString();
            }
            else jsonData = response.Content;

            JObject jsonObj = JObject.Parse(jsonData);

            var props = jsonObj.Properties().Select(x => x.Name).ToList();
            var values = jsonObj.Values().Select(x => x.ToString()).ToList();

            return new Dictionary<string, object>
                {
                    { "properties", props },
                    { "values", values }
                };
        }

        #endregion
    }
}
