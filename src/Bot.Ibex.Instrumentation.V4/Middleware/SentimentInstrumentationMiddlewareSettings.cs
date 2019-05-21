﻿namespace Bot.Ibex.Instrumentation.V4.Middleware
{
    using Bot.Ibex.Instrumentation.Common.Settings;

    public class SentimentInstrumentationMiddlewareSettings
    {
        public SentimentInstrumentationMiddlewareSettings(InstrumentationSettings instrumentationSettings, SentimentClientSettings sentimentClientSettings)
        {
            this.InstrumentationSettings = instrumentationSettings ?? throw new System.ArgumentNullException(nameof(instrumentationSettings));
            this.SentimentClientSettings = sentimentClientSettings ?? throw new System.ArgumentNullException(nameof(sentimentClientSettings));
        }

        public InstrumentationSettings InstrumentationSettings { get; }

        public SentimentClientSettings SentimentClientSettings { get; }
    }
}