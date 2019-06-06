namespace Bot.Instrumentation.V4.Instrumentations
{
    using System;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Settings;
    using Bot.Instrumentation.V4.Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.AI.QnA;
    using Microsoft.Bot.Schema;

    public class QnAInstrumentation : IQnAInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public QnAInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackEvent(IMessageActivity activity, QueryResult queryResult)
        {
            BotAssert.ActivityNotNull(activity);

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
