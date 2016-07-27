namespace CallTheWeb.ApiSelector
{
    using System.Collections.Generic;
    using System.Linq;

    public class WolframSelector
    {
        /// <summary>
        /// The fields
        /// </summary>
        private readonly string[] _keysRegex = {"Result", "facts", "Definition", "Basic information"};

        public string Select(IDictionary<string, string> content)
        {
            // Get the first field that matches a predefined regex,
            // or a second field in general if nothing found.
            var keys = content.Select(pair => pair.Key);
            var values = _keysRegex
                .Select(regex => keys.FirstOrDefault(key => key.Contains(regex)));
            return content[values.FirstOrDefault(value => !string.IsNullOrEmpty(value)) ?? content.Keys.ElementAt(1)];
        }
    }
}