using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication2.Translation
{
    public class TranslationQuestionHelper
    {
        private static readonly List<string> PossiblePrefixes = new List<string>
                                                                    {
                                                                        "how to say ",
                                                                        "how do you say ",
                                                                        "how do we say ",
                                                                        "how do I say",
                                                                        "what's ",
                                                                        "what is ",
                                                                        "translate "
                                                                    };

        private static readonly List<string> PossibleSuffixes = new List<string> { "in ", "to " };

        private static readonly List<string> LanguagesSupported =
            MicrosoftTranslationProvider.LanguageToFriendlyName.Keys.ToList();

        private static readonly List<string> PossibleLanguageSuffixes =
            PossibleSuffixes.SelectMany(s => LanguagesSupported, (x, y) => x + y).ToList();

        // We assume that question is in format: {prefix} {phrase text} {language suffix}
        public static bool IsQuestionTranslationRelated(string question)
        {
            var lowerCaseQuestion = question.ToLowerInvariant();
            var prefix =
                PossiblePrefixes.FirstOrDefault(
                    p => lowerCaseQuestion.IndexOf(p, StringComparison.InvariantCultureIgnoreCase) >= 0);
            var suffix =
                PossibleLanguageSuffixes.FirstOrDefault(
                    s => lowerCaseQuestion.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) >= 0);
            var notRelated = String.IsNullOrEmpty(prefix) || String.IsNullOrEmpty(suffix)
                             || lowerCaseQuestion.IndexOf(suffix) - lowerCaseQuestion.IndexOf(prefix) < 4;
            return !notRelated;
        }

        public static Tuple<string, string> GetQuestionLanguageAndText(string question)
        {
            var lowerCaseQuestion = question.ToLowerInvariant();
            var languageIndex = LanguagesSupported.Max(l => lowerCaseQuestion.IndexOf(l));
            var languageToTranslate = LanguagesSupported.Single(l => lowerCaseQuestion.IndexOf(l) == languageIndex);
            var prefixIndex =
                PossiblePrefixes.Where(p => lowerCaseQuestion.IndexOf(p) >= 0).Min(p => lowerCaseQuestion.IndexOf(p));
            var prefix = PossiblePrefixes.Single(p => lowerCaseQuestion.IndexOf(p) == prefixIndex);
            var cutPrefixUpTo = prefix.Length + prefixIndex;
            return new Tuple<string, string>(
                languageToTranslate,
                lowerCaseQuestion.Remove(languageIndex - 3).Remove(0, cutPrefixUpTo)); // cut language suffix and prefix
        }
    }
}