using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.ProjectOxford.SpeechRecognition;

namespace CallTheWeb.VoiceRecognision
{
    public class BingVoiceRecognition : IVoiceRecognition
    {
        private const string SubscriptionKey = "b3bfa345aad94efd8d449a87fd9ecaa7";

        private DataRecognitionClient client;

        private volatile bool waitForResponse;

        private string result;

        private string DefaultLocale
        {
            get { return "en-US"; }
        }

        public string ParseVoiceMessage(string recordingURL)
        {
            this.client = SpeechRecognitionServiceFactory.CreateDataClient(
                SpeechRecognitionMode.ShortPhrase, DefaultLocale, SubscriptionKey);

            this.client.OnResponseReceived += this.OnDataReceived;
            this.client.OnConversationError += this.OnErrorReceived;

            waitForResponse = true;

            this.SendAudioHelper(recordingURL);

            while (waitForResponse)
            {
                System.Threading.Thread.Sleep(100);
            }

            return result;
        }

        private void SendAudioHelper(string recodingURL)
        {
            using (var wc = new WebClient())
            using (Stream fileStream = wc.OpenRead(recodingURL))
            {
                int bytesRead = 0;
                byte[] buffer = new byte[1024];

                try
                {
                    do
                    {
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                        this.client.SendAudio(buffer, bytesRead);
                    }
                    while (bytesRead > 0);
                }
                finally
                {
                    this.client.EndAudio();
                }
            }
        }

        private void OnDataReceived(object sender, SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.Results.Length == 0)
            {
                result = "No result available";
            }

            result = e.PhraseResponse.Results[0].DisplayText;

            waitForResponse = false;
        }

        private void OnErrorReceived(object sender, SpeechErrorEventArgs e)
        {
            result = e.SpeechErrorText.ToString();

            waitForResponse = false;
        }
    }
}