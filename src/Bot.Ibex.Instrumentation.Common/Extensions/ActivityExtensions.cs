namespace Bot.Ibex.Instrumentation.Common.Extensions
{
    using System.Collections.Generic;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
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
    }
}
