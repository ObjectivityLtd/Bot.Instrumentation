namespace Bot.Ibex.Instrumentation.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Bot.Ibex.Instrumentation.Common.Sentiments;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

    public static class ActivityExtensions
    {
        public static bool IsIncomingMessage(this IActivityAdapter activity)
        {
            return activity?.Type == ActivityTypes.Message && activity?.ReplyToId == null;
        }

        public static MultiLanguageBatchInput ToSentimentInput(this IActivityAdapter activity)
        {
            return activity == null
                ? null
                : new MultiLanguageBatchInput(
                    new List<MultiLanguageInput>
                    {
                        new MultiLanguageInput
                        {
                            // TODO: investigate the following Language = activity.Locale,
                            Id = "1",
                            Text = activity.MessageActivity.Text,
                        },
                    });
        }

        public static void TrackCustomEvent(this IActivityAdapter activity, TelemetryClient telemetryClient, InstrumentationSettings settings, string eventName = EventTypes.CustomEvent, IDictionary<string, string> properties = null)
        {
            if (telemetryClient is null)
            {
                throw new ArgumentNullException(nameof(telemetryClient));
            }

            var builder = new EventTelemetryBuilder(activity, settings, properties);
            var eventTelemetry = builder.Build();
            eventTelemetry.Name = string.IsNullOrWhiteSpace(eventName)
                ? EventTypes.CustomEvent
                : eventName;

            telemetryClient.TrackEvent(eventTelemetry);
        }

        public static void TrackIntent(this IActivityAdapter activity, IntentResult result, TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (telemetryClient is null)
            {
                throw new ArgumentNullException(nameof(telemetryClient));
            }

            var properties = new Dictionary<string, string>
            {
                { IntentConstants.Intent, result.Intent },
                { IntentConstants.Score, result.Score },
                { IntentConstants.Entities, result.Entities },
            };

            activity.TrackCustomEvent(telemetryClient, settings, EventTypes.Intent, properties);
        }

        public static void TrackEvent(this IActivityAdapter activity, QueryResult queryResult, InstrumentationSettings settings, TelemetryClient telemetryClient)
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

            activity.TrackCustomEvent(telemetryClient, settings, EventTypes.QnaEvent, properties);
        }

        public static async Task TrackMessageSentiment(this IActivityAdapter activity, TelemetryClient telemetryClient, InstrumentationSettings settings, ISentimentClient sentimentClient)
        {
            if (telemetryClient is null)
            {
                throw new ArgumentNullException(nameof(telemetryClient));
            }

            if (sentimentClient is null)
            {
                throw new ArgumentNullException(nameof(sentimentClient));
            }

            var score = await sentimentClient.GetSentiment(activity)
                .ConfigureAwait(false);
            var properties = new Dictionary<string, string>
            {
                { SentimentConstants.Score, score.Value.ToString(CultureInfo.InvariantCulture) },
            };

            activity.TrackCustomEvent(telemetryClient, settings, EventTypes.MessageSentiment, properties);
        }
    }
}
