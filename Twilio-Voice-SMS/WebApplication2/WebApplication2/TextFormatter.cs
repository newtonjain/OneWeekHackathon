using System;
namespace WebApplication2
{
    using System.Collections.Generic;

    public class TextFormatter
    {
        /// <summary>
        /// Regex for string replacement. Order is important.
        /// </summary>
        private static Dictionary<string, string> ReplaceStringWith = new Dictionary<string, string>
        {
            { "\\n", "; " },
            { "  ", " " },
            { " |", ":" },
            { "\"", string.Empty }            
        };

        /// <summary>
        /// Beautifies string.
        /// </summary>
        /// <param name="stringToBeautify"></param>
        /// <returns></returns>
        public static string BeautifyString(string stringToBeautify)
        {
            foreach (var pair in ReplaceStringWith)
                stringToBeautify = stringToBeautify.Replace(pair.Key, pair.Value);
            return stringToBeautify;
        }

    }
}