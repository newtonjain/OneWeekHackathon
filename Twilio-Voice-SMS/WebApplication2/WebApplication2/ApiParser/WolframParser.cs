namespace CallTheWeb.ApiParser
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class WolframParser
    {
        /// <summary>
        /// Parses the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Parse(WebResponse response)
        {
            var content = await (new StreamReader(response.GetResponseStream()).ReadToEndAsync());
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        }
    }
}