using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

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
    }
}
