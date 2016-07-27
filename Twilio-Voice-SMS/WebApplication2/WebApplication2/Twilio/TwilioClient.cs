namespace WebApplication2.Twilio
{
    using global::Twilio;

    public class TwilioClient
    {
        private const string AccountSid = "ACfbcc4f9c0a7d62d23ed210d0e678e811";
        private const string AuthToken = "f51b660b1e80027213a6d1c8e458a5c0";

        /// <summary>
        /// Gets the Twilio client.
        /// </summary>
        /// <returns></returns>
        public static TwilioRestClient GetClient()
        {
            return new TwilioRestClient(AccountSid, AuthToken);
        }
    }
}