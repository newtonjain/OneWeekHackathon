namespace CallTheWeb.ApiRetriever
{
    using System.Net;
    using System.Threading.Tasks;

    public class WolframApi: IApi
    {
        /// <summary>
        /// The URL
        /// </summary>
        private const string Url = "http://konectivity.azurewebsites.net/hello?thebigquestion=";

        /// <summary>
        /// Gets the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public async Task<WebResponse> Get(string query)
        {
            var request = WebRequest.Create(Url+query);
            var response = await request.GetResponseAsync();
            return response;
        }
    }
}