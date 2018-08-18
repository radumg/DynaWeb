using System;
using System.Collections.Generic;
using System.Linq;
using DynaWeb.Properties;
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynaWeb
{
    /// <summary>
    /// A set of utility methods used throughout the DynaWeb package.
    /// </summary>
    public static class Helpers
    {
        #region URL & Uri handling
        /// <summary>
        /// Constructs a valid URI from a supplied string URL. Use this to both check and ensure URLs are valid
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <returns>A valid URI if the string URL is valid, throws Exception if not.</returns>
        public static Uri ParseUriFromString(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(Resources.WebUrlNullMessage);
            }

            Uri uriResult;
            var result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!result)
            {
                throw new UriFormatException(Resources.WebUrlInvalidMessage);
            }

            return uriResult;
        }

        /// <summary>Check the URI is valid</summary>
        /// <param name="uriToCheck">The URI to check</param>
        /// <returns>True if is valid, False otherwise</returns>
        public static Boolean IsUrlValid(Uri uriToCheck)
        {
            if (uriToCheck.IsFile || uriToCheck.IsUnc) throw new Exception(Resources.WebUrlIsLocal);

            if (!Uri.CheckSchemeName(uriToCheck.Scheme))
                return false;
            return true;
        }

        #endregion

        #region Deserialisation

        /// <summary>
        /// Recursively parse a JSON token into native data types.
        /// This includes all children of the JSON object, regardless of how many levels of nesting there are.
        /// </summary>
        /// <param name="json">The JSON token (object) to parse.</param>
        /// <returns>The parsed object</returns>
        public static object Deserialise(string json)
        {
            return ParseObject(JToken.Parse(json));
        }

        /// <summary>
        /// Deserialises a JSON string to a .NET object.
        /// </summary>
        /// <param name="json">The JSON string that needs to be deserialised.</param>
        /// <returns>The response deserialised as an object.</returns>
        public static dynamic DeserialiseAsObject(string json)
        {
            /// We don't want the deserialisation to break if some properties are empty.
            /// So we need to specify the behaviour when such values are encountered.
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.CheckAdditionalContent = true;

            return JsonConvert.DeserializeObject(json, settings);
        }

        /// <summary>
        /// Deserialises a JSON string to the type of a supplied object.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize to.</typeparam>
        /// <param name="json">The JSON string that needs to be deserialised.</param>
        /// <param name="obj">The object that will be used to determine what type to deserialise to.</param>
        /// <returns>The response deserialised as same type as supplied object.</returns>
        public static dynamic DeserialiseByObjectType(string json, object obj)
        {
            /// We don't want the deserialisation to break if some properties are empty.
            /// So we need to specify the behaviour when such values are encountered.
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.CheckAdditionalContent = true;
            var type = obj.GetType();

            return JsonConvert.DeserializeObject(json, type, settings);
        }

     /// <summary>
        /// Deserialises a JSON string into a dictionary of string keys and object values.
        /// Note : Does not handle deserialisation of nested objects.
        /// </summary>
        /// <param name="json">The JSON string to deserialise</param>
        /// <returns>A dictionary of the responses's JSON content.</returns>
        [MultiReturn(new[] { "properties", "values" })]
        public static Dictionary<string, object> DeserialiseAsDictionary(string json)
        {
            JObject jsonObj = JObject.Parse(json);

            var props = jsonObj.Properties().Select(x => x.Name).ToList();
            var values = jsonObj.Values().Select(x => x.ToString()).ToList();

            return new Dictionary<string, object>
                {
                    { "properties", props },
                    { "values", values }
                };
        }

        /// <summary>
        /// Serialises an object string to a JSON string.
        /// </summary>
        /// <param name="obj">The object that will be serialised.</param>
        /// <returns>Object serialised as JSON string.</returns>
        public static string SerialiseToJSON(object obj)
        {
            /// We don't want the serialisation to break if some properties are empty.
            /// So we need to specify the behaviour when such values are encountered.
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.CheckAdditionalContent = true;
            settings.Formatting = Formatting.Indented;

            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// Builds a new JSON string from the given root of an existing JSON object.
        /// </summary>
        /// <param name="json">The existing JSON</param>
        /// <param name="root">The name of the root object to return as JSON.</param>
        /// <returns>The new JSON string</returns>
        public static string SelectJsonRoot(string json, string root)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(root)) throw new ArgumentNullException();
            return JObject.Parse(json).SelectToken(root).ToString();
        }

        /// <summary>
        /// This method will recursively parse a JSON token into native .NET types.
        /// </summary>
        /// <param name="token">The JSON token (object) to parse.</param>
        /// <returns>The parsed object.</returns>
        private static object ParseObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name,
                                              prop => ParseObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ParseObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }

        #endregion
    }
}
