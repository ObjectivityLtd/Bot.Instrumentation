namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Collections.Generic;
    using Constants;
    using Microsoft.ApplicationInsights;
    using Settings;
    using Telemetry;

    public class QnAInstrumentation : IQnAInstrumentation
    {
        public const string QuestionsSeparator = ",";

        public void TrackEvent(IActivity activity, QueryResult queryResult, InstrumentationSettings settings, TelemetryClient telemetryClient)
        {
            var properties = new Dictionary<string, string>
            {
                { QnAConstants.UserQuery, activity.MessageActivity.Text },
                { QnAConstants.KnowledgeBaseQuestion, queryResult.KnowledgeBaseQuestion },
                { QnAConstants.KnowledgeBaseAnswer, queryResult.KnowledgeBaseAnswer },
                { QnAConstants.Score, queryResult.Score }
            };

            TrackTelemetry(activity, settings, telemetryClient, properties);
        }

        private static void TrackTelemetry(IActivity activity, InstrumentationSettings settings,
            TelemetryClient telemetryClient, Dictionary<string, string> properties)
        {
            var builder = new EventTelemetryBuilder(activity, settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.QnaEvent;
            telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
