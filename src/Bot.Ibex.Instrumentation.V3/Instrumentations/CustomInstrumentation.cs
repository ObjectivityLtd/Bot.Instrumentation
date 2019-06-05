namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Bot.Ibex.Instrumentation.V3.Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;

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
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            var objActivity = new ActivityAdapter(activity);

            var customInstrumentation =
                new Bot.Ibex.Instrumentation.Common.Instrumentations.CustomInstrumentation();
            customInstrumentation.TrackCustomEvent(objActivity, this.telemetryClient, this.settings, eventName, properties);
        }
    }
}
