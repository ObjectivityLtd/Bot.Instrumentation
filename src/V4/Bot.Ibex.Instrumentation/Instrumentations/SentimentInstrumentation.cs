namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System;
    using System.Threading.Tasks;
    using Bot.Ibex.Instrumentation.Common.Sentiments;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.V4.Adapters;
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

            var objActivity = new ActivityAdapter(activity);

            var sentimentInstrumentation =
                new Bot.Ibex.Instrumentation.Common.Instrumentations.SentimentInstrumentation();
            await sentimentInstrumentation.TrackMessageSentiment(objActivity, this.telemetryClient, this.settings, this.sentimentClient).ConfigureAwait(false);
        }
    }
}
