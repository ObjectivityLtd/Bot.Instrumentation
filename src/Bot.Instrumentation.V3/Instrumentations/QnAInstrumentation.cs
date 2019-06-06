namespace Bot.Instrumentation.V3.Instrumentations
{
    using System;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Settings;
    using Bot.Instrumentation.V3.Adapters;
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

            var activityAdapter = new ActivityAdapter(activity);
            var queryResultAdapter = new QueryResultAdapter(queryResult);

            activityAdapter.TrackEvent(queryResultAdapter.QueryResult, this.settings, this.telemetryClient);
        }
    }
}
