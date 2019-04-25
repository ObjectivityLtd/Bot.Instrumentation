﻿namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Settings;
    using Telemetry;

    public class CustomInstrumentation : ICustomInstrumentation
    {
        public void TrackCustomEvent(
            IActivity activity,
            TelemetryClient telemetryClient,
            InstrumentationSettings settings,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null)
        {
            TrackTelemetry(activity, telemetryClient, settings, eventName, properties);
        }

        private static void TrackTelemetry(
            IActivity activity,
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
