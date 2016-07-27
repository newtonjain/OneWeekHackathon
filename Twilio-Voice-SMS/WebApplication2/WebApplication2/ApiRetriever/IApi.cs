namespace CallTheWeb.ApiRetriever
{
    using System.Net;
    using System.Threading.Tasks;

    interface IApi
    {
        /// <summary>
        /// Gets the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        Task<WebResponse> Get(string query);
    }
}
