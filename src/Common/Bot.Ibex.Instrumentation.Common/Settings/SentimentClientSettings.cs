namespace Bot.Ibex.Instrumentation.Common.Settings
{
    using System;

    [Serializable]
    public class SentimentClientSettings
    {
        public string ApiSubscriptionKey { get; set; }

        public string Endpoint { get; set; }
    }
}
