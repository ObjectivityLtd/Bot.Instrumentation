﻿namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System;
    using Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;

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
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            var objectivityActivity = new ActivityAdapter(activity);
            var recognizerResultAdapter = new RecognizerResultAdapter(result);
            var convertedResult = recognizerResultAdapter.ConvertRecognizerResultToRecognizedIntentResult();

            var intentInstrumentation =
                new Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations.IntentInstrumentation();
            intentInstrumentation.TrackIntent(objectivityActivity, convertedResult, this.telemetryClient, this.settings);
        }
    }
}