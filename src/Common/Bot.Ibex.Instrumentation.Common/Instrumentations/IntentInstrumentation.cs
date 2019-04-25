﻿namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Models;
    using Settings;
    using Telemetry;

    public class IntentInstrumentation : IIntentInstrumentation
    {
        public void TrackIntent(IActivity activity, IntentResult result, TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            var properties = new Dictionary<string, string>
            {
                { IntentConstants.Intent, result.Intent },
                { IntentConstants.Score, result.Score },
                { IntentConstants.Entities, result.Entities }
            };

            TrackTelemetry(activity, properties, settings, telemetryClient);
        }

        private void TrackTelemetry(
            IActivity activity,
            IDictionary<string, string> properties,
            InstrumentationSettings settings,
            TelemetryClient telemetryClient)
        {
            var builder = new EventTelemetryBuilder(activity, settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.Intent;

            telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}