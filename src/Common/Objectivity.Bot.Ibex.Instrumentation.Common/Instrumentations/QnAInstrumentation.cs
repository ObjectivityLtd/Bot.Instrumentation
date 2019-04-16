namespace Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Constants;
    using Microsoft.ApplicationInsights;
    using Settings;
    using Telemetry;

    public class QnAInstrumentation : IQnAInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public QnAInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackEvent(IActivity activity, QueryResult queryResult)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }
            if (queryResult == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            var properties = new Dictionary<string, string>
            {
                { QnAConstants.UserQuery, activity.MessageActivity.Text },
                { QnAConstants.KnowledgeBaseQuestion, queryResult.KnowledgeBaseQuestion },
                { QnAConstants.KnowledgeBaseAnswer, queryResult.KnowledgeBaseAnswer },
                { QnAConstants.Score, queryResult.Score.ToString(CultureInfo.InvariantCulture) }
            };

            var builder = new EventTelemetryBuilder(activity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.QnaEvent;
            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
