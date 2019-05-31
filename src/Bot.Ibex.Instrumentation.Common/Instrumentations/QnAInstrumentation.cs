namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Microsoft.ApplicationInsights;

    public class QnAInstrumentation : IQnAInstrumentation
    {
        public void TrackEvent(IActivityAdapter activity, QueryResult queryResult, InstrumentationSettings settings, TelemetryClient telemetryClient)
        {
            if (activity is null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            if (queryResult is null)
            {
                throw new ArgumentNullException(nameof(queryResult));
            }

            if (telemetryClient is null)
            {
                throw new ArgumentNullException(nameof(telemetryClient));
            }

            var properties = new Dictionary<string, string>
            {
                { QnAConstants.UserQuery, activity.MessageActivity.Text },
                { QnAConstants.KnowledgeBaseQuestion, queryResult.KnowledgeBaseQuestion },
                { QnAConstants.KnowledgeBaseAnswer, queryResult.KnowledgeBaseAnswer },
                { QnAConstants.Score, queryResult.Score },
            };

            TrackTelemetry(activity, settings, telemetryClient, properties);
        }

        private static void TrackTelemetry(IActivityAdapter activity, InstrumentationSettings settings, TelemetryClient telemetryClient, Dictionary<string, string> properties)
        {
            var builder = new EventTelemetryBuilder(activity, settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.QnaEvent;
            telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
