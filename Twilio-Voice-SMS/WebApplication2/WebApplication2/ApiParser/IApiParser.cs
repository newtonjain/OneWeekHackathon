namespace CallTheWeb.ApiParser
{
    using System.Net;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    interface IApiParser
    {
        /// <summary>
        /// Parses the specified response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        Dictionary<string, string> Parse(WebResponse response);
    }
}