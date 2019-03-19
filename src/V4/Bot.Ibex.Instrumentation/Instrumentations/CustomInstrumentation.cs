namespace Bot.Ibex.Instrumentation.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Telemetry;

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
            IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null)
        {
            BotAssert.ActivityNotNull(activity);

            var builder = new EventTelemetryBuilder(activity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = string.IsNullOrWhiteSpace(eventName) ? EventTypes.CustomEvent : eventName;
            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
