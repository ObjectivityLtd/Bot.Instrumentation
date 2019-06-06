namespace Bot.Instrumentation.V3.Instrumentations
{
    using System;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Settings;
    using Bot.Instrumentation.V3.Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;

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
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var activityAdapter = new ActivityAdapter(activity);
            var luisResultAdapter = new LuisResultAdapter(result);

            activityAdapter.TrackIntent(luisResultAdapter.IntentResult, this.telemetryClient, this.settings);
        }
    }
}
