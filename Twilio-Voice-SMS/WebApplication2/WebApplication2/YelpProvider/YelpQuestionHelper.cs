using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CallTheWeb.Yelp
{
    public class YelpQuestionHelper
    {
        private static readonly List<string> PossibleQuestionPrefixes = new List<string>
                                                                    {
                                                                        "where is ",
                                                                        "give me ",
                                                                        "what is ",
                                                                        "where are "
                                                                    };

        private static readonly List<string> PossibleSuffixes = new List<string> { " in ", " near " };

        private static readonly List<string> YelpQuestionPrefixes = new List<string>
                                                                    {
                                                                        "Places - ",
                                                                        "Places: ",
                                                                        "Places "
                                                                    };

        public static bool IsYelpRequest(string request)
        {
            var lowerCaseRequest = request.ToLowerInvariant();
            var prefix =
                YelpQuestionPrefixes.FirstOrDefault(
                    p => lowerCaseRequest.IndexOf(p, StringComparison.InvariantCultureIgnoreCase) >= 0);

            return !String.IsNullOrEmpty(prefix);
        }

        public static bool IsRequestCorrectlyFormatted(string request)
        {
            var lowerCaseQuestion = request.ToLowerInvariant();
            var prefix =
                PossibleQuestionPrefixes.FirstOrDefault(
                    p => lowerCaseQuestion.IndexOf(p, StringComparison.InvariantCultureIgnoreCase) >= 0);
            var suffix =
                PossibleSuffixes.FirstOrDefault(
                    s => lowerCaseQuestion.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) >= 0);
            var notRelated = String.IsNullOrEmpty(prefix) || String.IsNullOrEmpty(suffix)
                             || lowerCaseQuestion.IndexOf(suffix) - lowerCaseQuestion.IndexOf(prefix) < 4;
            return !notRelated;
        }

        public static Tuple<string, string> GetRequestTermAndPlace(string request)
        {
            var lowerCaseQuestion = request.ToLowerInvariant();

            var suffixIndex = PossibleSuffixes.Max(l => lowerCaseQuestion.IndexOf(l));
            var suffix = PossibleSuffixes.Single(p => lowerCaseQuestion.IndexOf(p) == suffixIndex);

            var locationIndex = suffixIndex + suffix.Length;

            var locationString = lowerCaseQuestion.Remove(0, locationIndex);

            var prefixIndex = PossibleQuestionPrefixes.Max(l => lowerCaseQuestion.IndexOf(l));
            var prefix = PossibleQuestionPrefixes.Single(p => lowerCaseQuestion.IndexOf(p) == prefixIndex);

            var termIndex = prefixIndex + prefix.Length;
            var termString = lowerCaseQuestion.Remove(suffixIndex).Remove(0, termIndex);

            return new Tuple<string, string>(termString, locationString);
        }
    }
}