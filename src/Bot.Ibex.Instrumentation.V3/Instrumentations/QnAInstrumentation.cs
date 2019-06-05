﻿namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.V3.Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Microsoft.Bot.Connector;

    public class QnAInstrumentation : IQnAInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public QnAInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackEvent(IActivity activity, QnAMakerResults queryResult)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            if (queryResult == null)
            {
                throw new ArgumentNullException(nameof(queryResult));
            }

            var objActivity = new ActivityAdapter(activity);
            var queryResultAdapter = new QueryResultAdapter(queryResult);
            var result = queryResultAdapter.QueryResult;

            var qnaInstrumentation =
                new Bot.Ibex.Instrumentation.Common.Instrumentations.QnAInstrumentation();
            qnaInstrumentation.TrackEvent(objActivity, result, this.settings, this.telemetryClient);
        }
    }
}
