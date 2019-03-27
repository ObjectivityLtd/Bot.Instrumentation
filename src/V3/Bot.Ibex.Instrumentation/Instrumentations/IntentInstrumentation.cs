namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using Telemetry;

    [Serializable]
    public class IntentInstrumentation : IIntentInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public IntentInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackIntent(IActivity activity, LuisResult result)
        {
            if (result?.TopScoringIntent == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var properties = new Dictionary<string, string>
            {
                { IntentConstants.Intent, result.TopScoringIntent.Intent },
                { IntentConstants.Score, result.TopScoringIntent.Score.ToString() },
                { IntentConstants.Entities, JsonConvert.SerializeObject(result.Entities) }
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
