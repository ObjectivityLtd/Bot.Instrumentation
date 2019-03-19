namespace Bot.Ibex.Instrumentation.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;
    using Telemetry;

    public class IntentInstrumentation : IIntentInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public IntentInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackIntent(IActivity activity, RecognizerResult result)
        {
            BotAssert.ActivityNotNull(activity);

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var topScoringIntent = result.GetTopScoringIntent();

            var properties = new Dictionary<string, string>
            {
                { IntentConstants.Intent, topScoringIntent.intent },
                { IntentConstants.Score, topScoringIntent.score.ToString(CultureInfo.InvariantCulture) },
                { IntentConstants.Entities, result.Entities.ToString(Formatting.None) }
            };

            this.TrackIntent(activity, properties);
        }

        private void TrackIntent(IActivity activity, IDictionary<string, string> properties)
        {
            var builder = new EventTelemetryBuilder(activity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.Intent;

            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}