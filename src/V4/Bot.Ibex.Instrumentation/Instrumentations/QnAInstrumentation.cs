namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Adapters;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.AI.QnA;
    using Microsoft.Bot.Schema;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;

    public class QnAInstrumentation : IQnAInstrumentation
    {
        public const string QuestionsSeparator = ",";

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

            var properties = new Dictionary<string, string>
            {
                { QnAConstants.UserQuery, activity.Text },
                { QnAConstants.KnowledgeBaseQuestion, string.Join(QuestionsSeparator, queryResult.Questions) },
                { QnAConstants.KnowledgeBaseAnswer, queryResult.Answer },
                { QnAConstants.Score, queryResult.Score.ToString(CultureInfo.InvariantCulture) }
            };

            var objectivityActivity = new ActivityAdapter(activity);
            var builder = new EventTelemetryBuilder(objectivityActivity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.QnaEvent;
            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
