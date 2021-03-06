﻿namespace Bot.Instrumentation.V4.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Settings;
    using Bot.Instrumentation.Common.Telemetry;
    using Bot.Instrumentation.V4.Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public class CustomInstrumentation : ICustomInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public CustomInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackCustomEvent(IActivity activity, string eventName = EventTypes.CustomEvent, IDictionary<string, string> properties = null)
        {
            BotAssert.ActivityNotNull(activity);

            var activityAdapter = new ActivityAdapter(activity);
            activityAdapter.TrackCustomEvent(this.telemetryClient, this.settings, eventName, properties);
        }
    }
}
