namespace CallTheWeb.ApiSelector
{
    using System.Collections.Generic;

    interface IApiSelector
    {
        /// <summary>
        /// Selects the specified content map.
        /// </summary>
        /// <param name="contentMap">The content map.</param>
        /// <returns></returns>
        string Select(IDictionary<string, string> contentMap);
    }
}
