﻿namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.V4.Adapters;
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

            var objectivityActivity = new ActivityAdapter(activity);
            var recognizerResultAdapter = new RecognizerResultAdapter(result).IntentResult;

            var intentInstrumentation =
                new Bot.Ibex.Instrumentation.Common.Instrumentations.IntentInstrumentation();
            intentInstrumentation.TrackIntent(objectivityActivity, recognizerResultAdapter, this.telemetryClient, this.settings);
        }
    }
}