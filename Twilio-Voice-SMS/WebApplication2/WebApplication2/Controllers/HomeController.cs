namespace WebApplication2.Controllers
{
    using CallTheWeb.ApiParser;
    using CallTheWeb.ApiRetriever;
    using CallTheWeb.ApiSelector;
    using CallTheWeb.VoiceRecognision;
    using CallTheWeb.Yelp;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Xml;

    using Twilio;

    using WebApplication2.Translation;

    public class HomeController : Controller
    {
        /// <summary>
        /// Twilio registered number
        /// </summary>
        private const string TwilioNumber = "+15878033755";

        /// <summary>
        /// Return this string if no results found.
        /// </summary>
        private const string FailureMessage = "We have not found any results, please try again.";

        private static readonly Dictionary<string, string> LanguageToFriendlyTwilioName = 
            new Dictionary<string, string>{{"english", "en-US"},{"french","fr-FR"},{"german","de-DE"},
                { "italian","it-IT"},{"japanese","ja-JP"},{"korean","ko-KR"},{"polish","pl-PL"},
                { "portuguese","pt-PT"},{"russian","ru-RU"},{"spanish","es-ES"}};

        private static readonly List<string> AdminNumbers = new List<string>
                                                                {
                                                                    "+16046937094"
                                                                };

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ReceiveVoice()
        {
            Trace.TraceInformation("Receive Voice method begins");
            var question = TranscribeTheAudio();
            Trace.TraceInformation("Answering " + question);
            string answer;
            string languageFriendlyName = "en-US";
            if (TranslationQuestionHelper.IsQuestionTranslationRelated(question))
            {
                Trace.TraceInformation("Answering translation related question");
                var languageAndPhrase = TranslationQuestionHelper.GetQuestionLanguageAndText(question);
                answer = MicrosoftTranslationProvider.Translate(languageAndPhrase.Item2, languageAndPhrase.Item1);
                languageFriendlyName = LanguageToFriendlyTwilioName[languageAndPhrase.Item1];
            }
            else if (YelpQuestionHelper.IsYelpRequest(question))
            {
                if (YelpQuestionHelper.IsRequestCorrectlyFormatted(question))
                {
                    Tuple<string, string> termAndPlace = YelpQuestionHelper.GetRequestTermAndPlace(question);
                    answer = YelpApiProvider.GetYelpResponse(termAndPlace.Item1, termAndPlace.Item2);
                }
                else
                {
                    answer = "Question to Yelp is wrongly formatted. Please try again";
                }
            }
            else
            {
                Trace.TraceInformation("Answering general question");
                answer = await RetrieveWolphramAnswer(question);
            }
            Trace.TraceInformation($"The answer is {answer}");
            ViewBag.answer = answer;
            ViewBag.language = languageFriendlyName;
            ViewBag.isAdminFlow = AdminNumbers.Contains(Request["From"]);

            if (!answer.Equals(FailureMessage))
            {
                Trace.TraceInformation($"Sending message to {Request["From"]}");
                TwilioClient.GetClient().SendMessage(TwilioNumber, Request["From"], answer);
            }
            Trace.TraceInformation($"Returning ReceiveVoice View");
            return View();
        }

        private string TranscribeTheAudio()
        {
            IVoiceRecognition voiceRecognition = new BingVoiceRecognition();
            var question = voiceRecognition.ParseVoiceMessage(Request["RecordingUrl"]);
            return question;
        }

        [HttpPost]
        public async Task<ActionResult> ReceiveText()
        {
            // To invoke from PowerShell, copy-paste the following:
            /*
                $question = "What is the weather in Vancouver, BC?"
                Invoke-WebRequest -Uri ("http://localhost:1729/Home/ReceiveText?from=6048310085&test=true&body=" + $question) -Method POST
            */

            var from = Request["From"];
            var question = Request["Body"];
            Trace.TraceInformation("Answering " + question);
            var simulationMode = false;
            Boolean.TryParse(Request["Test"], out simulationMode);
            string answer;
            string xmlResult = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><Response><Text>"
                + question + "</Text></Response>";

            if (TranslationQuestionHelper.IsQuestionTranslationRelated(question))
            {
                var languageAndPhrase = TranslationQuestionHelper.GetQuestionLanguageAndText(question);
                answer = MicrosoftTranslationProvider.Translate(languageAndPhrase.Item2, languageAndPhrase.Item1);
            }
            else if (YelpQuestionHelper.IsYelpRequest(question))
            {
                if (YelpQuestionHelper.IsRequestCorrectlyFormatted(question))
                {
                    Tuple<string, string> termAndPlace = YelpQuestionHelper.GetRequestTermAndPlace(question);
                    answer = YelpApiProvider.GetYelpResponse(termAndPlace.Item1, termAndPlace.Item2);
                }
                else
                {
                    answer = "Question to Yelp is wrongly formatted. Please try again";
                }
            }
            else
            {
                answer = await RetrieveWolphramAnswer(question);
            }

            if (!simulationMode)
            {
                TwilioClient.GetClient().SendMessage(
                            TwilioNumber,
                            from,
                            answer
                    );
            }

            return this.Content(xmlResult, "text/xml");
        }

        /// <summary>
        /// Retrieves an answer to the question from wolphram alpha
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        private async Task<string> RetrieveWolphramAnswer(string question)
        {
            try
            {
                var wolframResponse = await (new WolframApi()).Get(question);
                var wolframContentMap = await (new WolframParser().Parse(wolframResponse));
                var wolframContent = new WolframSelector().Select(wolframContentMap);
                if (wolframContent.Contains("data not available"))
                    throw new Exception("No data found");
                return TextFormatter.BeautifyString(wolframContent);
            }
            catch
            {
                return FailureMessage;
            }
        }
    }
}
