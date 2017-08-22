using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynWWW.Classes
{
    /// <summary>
    /// The web client represents a uniquely configured connection to a server or service.
    /// </summary>
    public class WebClient
    {

        #region private members
        private RestClient restClient;
        private Uri baseUrl;
        private string token;
        #endregion

        #region public members
        public Uri BaseUrl { get => restClient.BaseUrl; set => restClient.BaseUrl = value; }
        public string JsonTokenOverride { get; set; }
        #endregion

        #region client settings
        public int? MaxRedirects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserAgent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Timeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ReadWriteTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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
            this.token = token;

        }

        #endregion

        #region methods

        #endregion
    }
}
