namespace Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Constants;
    using Microsoft.ApplicationInsights;
    using Sentiments;
    using Settings;
    using Telemetry;

    public class SentimentInstrumentation : ISentimentInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;
        private readonly ISentimentClient sentimentClient;

        public SentimentInstrumentation(InstrumentationSettings settings, TelemetryClient telemetryClient, ISentimentClient sentimentClient)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.sentimentClient = sentimentClient ?? throw new ArgumentNullException(nameof(sentimentClient));
        }

        public async Task TrackMessageSentiment(IActivity activity)
        {
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
