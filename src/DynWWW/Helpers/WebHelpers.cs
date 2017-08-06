using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynWWW.Properties;

namespace DynWWW.Helpers
{
    internal static class WebHelpers
    {
        /// <summary>
        /// Constructs a valid URI from a supplied string URL. Use this to both check and ensure URLs are valid
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <returns>A valid URI if the string URL is valid, throws Exception if not.</returns>
        public static Uri ParseUriFromString(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(Properties.Resources.WebRequestUrlNullMessage);
            }

            Uri uriResult;
            var result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp
                              || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!result)
            {
                throw new UriFormatException(Properties.Resources.WebRequestUrlInvalidMessage); 
            }

            return uriResult;
        }

    }
}
