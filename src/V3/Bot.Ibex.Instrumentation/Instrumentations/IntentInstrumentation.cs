namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using Adapters;
    using Bot.Ibex.Instrumentation.Common.Settings;
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

            var objectivityActivity = new ActivityAdapter(activity);
            var luisResultAdapter = new LuisResultAdapter(result);
            var convertedResult = luisResultAdapter.ConvertLuisResultToRecognizedIntentResult();

            var intentInstrumentation =
                new Bot.Ibex.Instrumentation.Common.Instrumentations.IntentInstrumentation();
            intentInstrumentation.TrackIntent(objectivityActivity, convertedResult, this.telemetryClient, this.settings);
        }
    }
}
