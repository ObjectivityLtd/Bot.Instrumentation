﻿namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Threading.Tasks;
    using Adapters;
    using Microsoft.ApplicationInsights;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;

    public class ActivityInstrumentation : IActivityInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public ActivityInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public Task TrackActivity(Microsoft.Bot.Connector.IActivity activity)
        {
            return Task.Run(() =>
            {
                var objectivityActivity = new ActivityAdapter(activity);
                var builder = new EventTelemetryBuilder(objectivityActivity, this.settings);
                var eventTelemetry = builder.Build();
                this.telemetryClient.TrackEvent(eventTelemetry);
            });
        }
    }
}
