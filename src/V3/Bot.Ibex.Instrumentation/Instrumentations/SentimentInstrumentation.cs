namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Threading.Tasks;
    using Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Sentiments;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;

    public class SentimentInstrumentation : ISentimentInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly ISentimentClient sentimentClient;
        private readonly InstrumentationSettings settings;

        public SentimentInstrumentation(InstrumentationSettings settings, TelemetryClient telemetryClient, ISentimentClient sentimentClient)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.sentimentClient = sentimentClient ?? throw new ArgumentNullException(nameof(sentimentClient));
        }

        public async Task TrackMessageSentiment(IActivity activity)
        {
            var objActivity = new ActivityAdapter(activity);

            var sentimentInstrumentation =
                new Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations.SentimentInstrumentation();
            await sentimentInstrumentation.TrackMessageSentiment(objActivity, this.telemetryClient, this.settings, this.sentimentClient).ConfigureAwait(false);
        }
    }
}
