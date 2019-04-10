namespace Bot.Ibex.Instrumentation.V3.ActivityLoggers
{
    using System;
    using System.Threading.Tasks;
    using Instrumentations;
    using Microsoft.Bot.Builder.History;
    using Microsoft.Bot.Connector;

    public class DialogActivityLogger : IActivityLogger
    {
        private readonly IActivityInstrumentation activityInstrumentation;

        public DialogActivityLogger(IActivityInstrumentation activityInstrumentation)
        {
            this.activityInstrumentation = activityInstrumentation ?? throw new ArgumentNullException(nameof(activityInstrumentation));
        }

        public async Task LogAsync(IActivity activity)
        {
            await this.activityInstrumentation.TrackActivity(activity)
                .ConfigureAwait(false);
        }
    }
}
