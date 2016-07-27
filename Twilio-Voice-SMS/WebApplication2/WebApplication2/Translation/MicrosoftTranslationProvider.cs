// ReSharper disable InconsistentNaming
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace WebApplication2.Translation
{
    public class MicrosoftTranslationProvider
    {
        private const string ClientId = "CallTheWeb";

        private const string ClientSecret = "dDzT1U6oPmc9+Vema31CoYgnBTGjec9p4mT1jGKWqAw=";

        private const string BaseTranslateUri = "http://api.microsofttranslator.com/v2/Http.svc/Translate";

        public static readonly Dictionary<string, string> LanguageToFriendlyName = new Dictionary<string, string>
                                                                                       {
                                                                                           {
                                                                                               "english",
                                                                                               "en"
                                                                                           },
                                                                                           {
                                                                                               "french",
                                                                                               "fr"
                                                                                           },
                                                                                           {
                                                                                               "german",
                                                                                               "de"
                                                                                           },
                                                                                           {
                                                                                               "italian",
                                                                                               "it"
                                                                                           },
                                                                                           {
                                                                                               "japanese",
                                                                                               "ja"
                                                                                           },
                                                                                           {
                                                                                               "korean",
                                                                                               "ko"
                                                                                           },
                                                                                           {
                                                                                               "polish",
                                                                                               "pl"
                                                                                           },
                                                                                           {
                                                                                               "portuguese",
                                                                                               "pt"
                                                                                           },
                                                                                           {
                                                                                               "russian",
                                                                                               "ru"
                                                                                           },
                                                                                           {
                                                                                               "spanish",
                                                                                               "es"
                                                                                           }
                                                                                       };

        public static string Translate(string phrase, string language)
        {
            if (!LanguageToFriendlyName.Keys.Contains(language.ToLowerInvariant()))
            {
                Trace.TraceError("No friendly name found for " + language);
                return null;
            }
            var translateUri = GetTranslateUri(
                LanguageToFriendlyName["english"],
                LanguageToFriendlyName[language],
                phrase.Trim());
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(translateUri);
            httpWebRequest.Headers.Add("Authorization", GetAccessToken());
            try
            {
                var response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    var dataContractSerializer = new DataContractSerializer(Type.GetType("System.String"));
                    return (string)dataContractSerializer.ReadObject(stream);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Can't get a translation for '" + phrase + "' with error " + e);
                return null;
            }
        }

        private static string GetTranslateUri(string languageFrom, string languageTo, string text)
        {
            return BaseTranslateUri + "?text=" + HttpUtility.UrlEncode(text) + "&from=" + languageFrom + "&to="
                   + languageTo;
        }

        private static string GetAccessToken()
        {
            var admAuth = new AdmAuthentication(ClientId, ClientSecret);
            string headerValue = null;
            try
            {
                var admToken = admAuth.GetAccessToken();
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            return headerValue;
        }
    }

    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }

        [DataMember]
        public string token_type { get; set; }

        [DataMember]
        public string expires_in { get; set; }

        [DataMember]
        public string scope { get; set; }
    }

    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";

        private readonly string clientId;

        private string clientSecret;

        private readonly string request;

        private AdmAccessToken token;

        private readonly Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            this.request =
                string.Format(
                    "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
                    HttpUtility.UrlEncode(clientId),
                    HttpUtility.UrlEncode(clientSecret));
            this.token = HttpPost(DatamarketAccessUri, this.request);
            //renew the token every specfied minutes
            accessTokenRenewer = new Timer(
                this.OnTokenExpiredCallback,
                this,
                TimeSpan.FromMinutes(RefreshTokenDuration),
                TimeSpan.FromMilliseconds(-1));
        }

        public AdmAccessToken GetAccessToken()
        {
            return this.token;
        }

        private void RenewAccessToken()
        {
            AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, this.request);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.token = newAccessToken;
            Trace.TraceInformation($"Renewed token for user: {this.clientId} is: {this.token.access_token}");
        }

        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Failed renewing access token. Details: {ex.Message}");
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Failed to reschedule the timer to renew access token. Details: {ex.Message}");
                }
            }
        }

        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
}