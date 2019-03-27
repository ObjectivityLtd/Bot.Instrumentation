namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using Sentiments;
    using Telemetry;

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

            var score = await this.sentimentClient.GetSentiment(activity)
                .ConfigureAwait(false);
            var properties = new Dictionary<string, string>
            {
                { SentimentConstants.Score, score.Value.ToString(CultureInfo.InvariantCulture) }
            };

            var builder = new EventTelemetryBuilder(activity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.MessageSentiment;
            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
