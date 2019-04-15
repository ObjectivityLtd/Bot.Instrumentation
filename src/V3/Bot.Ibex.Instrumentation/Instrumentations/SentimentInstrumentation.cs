namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Constants;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using Sentiments;
    using IActivity = Microsoft.Bot.Connector.IActivity;

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

            var objectivityActivity = new ActivityAdapter(activity);
            var builder = new EventTelemetryBuilder(objectivityActivity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.MessageSentiment;
            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
