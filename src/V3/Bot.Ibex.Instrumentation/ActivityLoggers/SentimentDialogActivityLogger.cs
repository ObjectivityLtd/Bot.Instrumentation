namespace Bot.Ibex.Instrumentation.V3.ActivityLoggers
{
    using System;
    using System.Threading.Tasks;
    using Adapters;
    using Instrumentations;
    using Microsoft.Bot.Builder.History;
    using Microsoft.Bot.Connector;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations;

    public class SentimentDialogActivityLogger : IActivityLogger
    {
        private readonly ISentimentInstrumentation sentimentInstrumentation;

        public SentimentDialogActivityLogger(ISentimentInstrumentation sentimentInstrumentation)
        {
            this.sentimentInstrumentation = sentimentInstrumentation ?? throw new ArgumentNullException(nameof(sentimentInstrumentation));
        }

        public async Task LogAsync(IActivity activity)
        {
            var objectivityActivity = new ActivityAdapter(activity);
            await this.sentimentInstrumentation.TrackMessageSentiment(objectivityActivity)
                .ConfigureAwait(false);
        }
    }
}
