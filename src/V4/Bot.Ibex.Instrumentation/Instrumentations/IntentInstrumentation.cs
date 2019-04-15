namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Newtonsoft.Json;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Constants;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;

    public class IntentInstrumentation : IIntentInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public IntentInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackIntent(Microsoft.Bot.Schema.IActivity activity, RecognizerResult result)
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

        private void TrackIntent(Microsoft.Bot.Schema.IActivity activity, IDictionary<string, string> properties)
        {
            var objectivityActivity = new ActivityAdapter(activity);
            var builder = new EventTelemetryBuilder(objectivityActivity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.Intent;

            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}