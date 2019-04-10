﻿namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;

    public class CustomInstrumentation : ICustomInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public CustomInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackCustomEvent(
            Microsoft.Bot.Schema.IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null)
        {
            BotAssert.ActivityNotNull(activity);

            var objectivityActivity = new ActivityAdapter(activity);
            var builder = new EventTelemetryBuilder(objectivityActivity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = string.IsNullOrWhiteSpace(eventName) ? EventTypes.CustomEvent : eventName;
            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
