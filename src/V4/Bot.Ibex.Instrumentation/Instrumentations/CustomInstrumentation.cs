namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Adapters;
    using Bot.Ibex.Instrumentation.Common.Constants;
    using Bot.Ibex.Instrumentation.Common.Settings;
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

            var objActivity = new ActivityAdapter(activity);

            var customInstrumentation =
                new Bot.Ibex.Instrumentation.Common.Instrumentations.CustomInstrumentation();
            customInstrumentation.TrackCustomEvent(objActivity, this.telemetryClient, this.settings, eventName, properties);
        }
    }
}
