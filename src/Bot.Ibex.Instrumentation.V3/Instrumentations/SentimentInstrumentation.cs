namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Threading.Tasks;
    using Bot.Ibex.Instrumentation.Common.Extensions;
    using Bot.Ibex.Instrumentation.Common.Sentiments;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.V3.Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;

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
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            var activityAdapter = new ActivityAdapter(activity);

            await activityAdapter.TrackMessageSentiment(this.telemetryClient, this.settings, this.sentimentClient).ConfigureAwait(false);
        }
    }
}
