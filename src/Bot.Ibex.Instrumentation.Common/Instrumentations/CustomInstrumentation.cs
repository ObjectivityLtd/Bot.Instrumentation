namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Microsoft.ApplicationInsights;

    public class CustomInstrumentation : ICustomInstrumentation
    {
        public void TrackCustomEvent(
            IActivityAdapter activity,
            TelemetryClient telemetryClient,
            InstrumentationSettings settings,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null)
        {
            if (telemetryClient is null)
            {
                throw new ArgumentNullException(nameof(telemetryClient));
            }

            TrackTelemetry(activity, telemetryClient, settings, eventName, properties);
        }

        private static void TrackTelemetry(
            IActivityAdapter activity,
            TelemetryClient telemetryClient,
            InstrumentationSettings settings,
            string eventName,
            IDictionary<string, string> properties)
        {
            var builder = new EventTelemetryBuilder(activity, settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = string.IsNullOrWhiteSpace(eventName) ? EventTypes.CustomEvent : eventName;
            telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
