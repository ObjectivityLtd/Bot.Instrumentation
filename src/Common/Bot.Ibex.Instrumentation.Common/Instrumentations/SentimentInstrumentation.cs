namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
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
        public async Task TrackMessageSentiment(IActivity activity, TelemetryClient telemetryClient, InstrumentationSettings settings, ISentimentClient sentimentClient)
        {
            var score = await sentimentClient.GetSentiment(activity)
                .ConfigureAwait(false);
            var properties = new Dictionary<string, string>
            {
                { SentimentConstants.Score, score.Value.ToString(CultureInfo.InvariantCulture) }
            };

            TrackTelemetry(activity, telemetryClient, settings, properties);
        }

        private static void TrackTelemetry(IActivity activity, TelemetryClient telemetryClient,
            InstrumentationSettings settings, Dictionary<string, string> properties)
        {
            var builder = new EventTelemetryBuilder(activity, settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.MessageSentiment;
            telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
