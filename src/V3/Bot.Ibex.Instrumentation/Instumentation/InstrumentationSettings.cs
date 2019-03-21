namespace Bot.Ibex.Instrumentation.Instumentation
{
    using Bot.Ibex.Instrumentation.Managers;
    using System.Collections.Generic;
    public class InstrumentationSettings
    {
        public List<string> InstrumentationKeys { get; set; }
        public SentimentManager SentimentManager { get; set; }
        public bool OmitUsernameFromTelemetry { get; set; }
    }
}
