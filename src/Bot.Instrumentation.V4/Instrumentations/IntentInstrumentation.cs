namespace Bot.Instrumentation.V4.Instrumentations
{
    using System;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Settings;
    using Bot.Instrumentation.V4.Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public class IntentInstrumentation : IIntentInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public IntentInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackIntent(IActivity activity, RecognizerResult result)
        {
            BotAssert.ActivityNotNull(activity);

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var activityAdapter = new ActivityAdapter(activity);
            var recognizerResultAdapter = new RecognizerResultAdapter(result);

            activityAdapter.TrackIntent(recognizerResultAdapter.IntentResult, this.telemetryClient, this.settings);
        }
    }
}