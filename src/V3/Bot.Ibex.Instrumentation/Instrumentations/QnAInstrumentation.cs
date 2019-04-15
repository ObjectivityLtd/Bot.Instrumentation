namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Adapters;
    using Microsoft.ApplicationInsights;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Constants;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using IActivity = Microsoft.Bot.Connector.IActivity;

    public class QnAInstrumentation : IQnAInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public QnAInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void TrackEvent(IActivity activity, string userQuery, string kbQuestion, string kbAnswer, double score)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            var properties = new Dictionary<string, string>
            {
                { QnAConstants.UserQuery, userQuery },
                { QnAConstants.KnowledgeBaseQuestion, kbQuestion },
                { QnAConstants.KnowledgeBaseAnswer, kbAnswer },
                { QnAConstants.Score, score.ToString(CultureInfo.InvariantCulture) }
            };

            var objectivityActivity = new ActivityAdapter(activity);
            var builder = new EventTelemetryBuilder(objectivityActivity, this.settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = EventTypes.QnaEvent;
            this.telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
