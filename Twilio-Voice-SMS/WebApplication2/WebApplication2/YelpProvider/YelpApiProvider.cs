using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

using Newtonsoft.Json.Linq;
using SimpleOAuth;

namespace CallTheWeb.Yelp
{
    public class YelpApiProvider
    {
        private const string CONSUMER_KEY = "8MXnTwWQSzfUBrW3YBk0Qw";

        private const string CONSUMER_SECRET = "N88iw7iQQvLI6uWzJp8Jk0UslXc";

        private const string TOKEN = "uTEzKNw1sLdHZMxcb2uIv1xC65HlDDe3";

        private const string TOKEN_SECRET = "R1sRfwPS2K_gCP8dWdU_OkoI2EI";

        private const string BASE_URL = "https://api.yelp.com/v2/search/";

        private const int RESULT_LIMIT = 3;

        public static string GetYelpResponse(string term, string location)
        {
            var uriBulder = new UriBuilder(BASE_URL);

            var query = System.Web.HttpUtility.ParseQueryString(String.Empty);

            var queryParams = new Dictionary<string, string>()
            {
                { "term", term },
                { "location", location },
                { "limit", RESULT_LIMIT.ToString() }
            };

            foreach (var param in queryParams)
            {
                query[param.Key] = param.Value;
            }

            uriBulder.Query = query.ToString();

            var request = WebRequest.Create(uriBulder.ToString());

            request.Method = "GET";

            request.SignRequest(
                new Tokens
                {
                    ConsumerKey = CONSUMER_KEY,
                    ConsumerSecret = CONSUMER_SECRET,
                    AccessToken = TOKEN,
                    AccessTokenSecret = TOKEN_SECRET
                }
            ).WithEncryption(EncryptionMethod.HMACSHA1).InHeader();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return ParseJsonToMessage(JObject.Parse(stream.ReadToEnd()));
        }

        private static string ParseJsonToMessage(JObject response)
        {
            JArray businessses = (JArray)response.GetValue("businesses");

            if (businessses.Count == 0)
            {
                return "No anwers found on Yelp";
            }

            var business = businessses[0];

            string businessName = (string)business["name"];
            string address = String.Format("{0} {1}", (string)business["location"]["address"][0], 
                (string)business["location"]["city"]);
            string averageRate = business["rating"].ToString();
            string phoneNumber = business["phone"].ToString();

            return String.Format("Best result found on Yelp: Name: {0} Rating: {1} Address: {2} Phone: {3}",
                businessName, averageRate, address, phoneNumber);
        }
    }
}