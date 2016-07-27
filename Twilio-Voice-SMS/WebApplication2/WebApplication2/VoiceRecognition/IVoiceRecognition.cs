using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallTheWeb.VoiceRecognision
{
    interface IVoiceRecognition
    {
        /// <summary>
        /// Parse a recording into text message
        /// </summary>
        /// <param name="recordingURL">The location of recording</param>
        /// <returns></returns>
        string ParseVoiceMessage(string recordingURL);
    }
}
