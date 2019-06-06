namespace Bot.Instrumentation.V4.Instrumentations
{
    using System;
    using System.Threading.Tasks;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Sentiments;
    using Bot.Instrumentation.Common.Settings;
    using Bot.Instrumentation.V4.Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public class SentimentInstrumentation : ISentimentInstrumentation
    {
        private readonly ISentimentClient sentimentClient;
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public SentimentInstrumentation(ISentimentClient sentimentClient, TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.sentimentClient = sentimentClient ?? throw new ArgumentNullException(nameof(sentimentClient));
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task TrackMessageSentiment(IMessageActivity activity)
        {
            BotAssert.ActivityNotNull(activity);

            var activityAdapter = new ActivityAdapter(activity);

            await activityAdapter.TrackMessageSentiment(this.telemetryClient, this.settings, this.sentimentClient).ConfigureAwait(false);
        }
    }
}
