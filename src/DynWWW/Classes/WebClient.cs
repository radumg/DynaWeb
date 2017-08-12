using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynWWW.Classes
{
    public class WebClient
    {

        #region private members
        private RestClient restClient;
        private Uri baseUrl;
        #endregion

        #region public members
        public int? MaxRedirects { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserAgent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Timeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ReadWriteTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Uri BaseUrl { get => baseUrl; set => baseUrl = value; }
        #endregion

        #region constructor

        public WebClient(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(DynWWW.Properties.Resources.WebClientUrlNullMessage);

            restClient = new RestClient(baseUrl);
        }

        #endregion

        #region methods

        #endregion
    }
}
