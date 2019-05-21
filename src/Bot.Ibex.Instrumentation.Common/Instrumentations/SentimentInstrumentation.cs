namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Bot.Ibex.Instrumentation.Common.Sentiments;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Microsoft.ApplicationInsights;

    public class SentimentInstrumentation : ISentimentInstrumentation
    {
        public async Task TrackMessageSentiment(IActivityAdapter activity, TelemetryClient telemetryClient, InstrumentationSettings settings, ISentimentClient sentimentClient)
        {
            if (telemetryClient is null)
            {
                throw new ArgumentNullException(nameof(telemetryClient));
            }

            if (sentimentClient is null)
            {
                throw new ArgumentNullException(nameof(sentimentClient));
            }

            var score = await sentimentClient.GetSentiment(activity)
                .ConfigureAwait(false);
            var properties = new Dictionary<string, string>
            {
                { SentimentConstants.Score, score.Value.ToString(CultureInfo.InvariantCulture) },
            };

            TrackTelemetry(activity, telemetryClient, settings, properties);
        }

        private static void TrackTelemetry(IActivityAdapter activity, TelemetryClient telemetryClient, InstrumentationSettings settings, Dictionary<string, string> properties)
        {
            var builder = new EventTelemetryBuilder(activity, settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.MessageSentiment;
            telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
