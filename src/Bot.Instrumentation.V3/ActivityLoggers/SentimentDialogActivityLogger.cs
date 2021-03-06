﻿namespace Bot.Instrumentation.V3.ActivityLoggers
{
    using System;
    using System.Threading.Tasks;
    using Bot.Instrumentation.V3.Instrumentations;
    using Microsoft.Bot.Builder.History;
    using Microsoft.Bot.Connector;

    public class SentimentDialogActivityLogger : IActivityLogger
    {
        private readonly ISentimentInstrumentation sentimentInstrumentation;

        public SentimentDialogActivityLogger(ISentimentInstrumentation sentimentInstrumentation)
        {
            this.sentimentInstrumentation = sentimentInstrumentation ?? throw new ArgumentNullException(nameof(sentimentInstrumentation));
        }

        public async Task LogAsync(IActivity activity)
        {
            await this.sentimentInstrumentation.TrackMessageSentiment(activity)
                .ConfigureAwait(false);
        }
    }
}
