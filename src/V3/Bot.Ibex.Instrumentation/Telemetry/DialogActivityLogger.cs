namespace Bot.Ibex.Instrumentation.Telemetry
{
    using Autofac;
    using Bot.Ibex.Instrumentation.Instumentation;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.History;
    using Microsoft.Bot.Connector;
    using System.Threading.Tasks;

    /// <summary>
    /// A generic logger for Dialog activities. 
    /// </summary>
    public class DialogActivityLogger : IActivityLogger
    {
        private readonly IBotData botData;

        public DialogActivityLogger(IBotData botData)
        {
            this.botData = botData;
        }

        public async Task LogAsync(IActivity activity)
        {
            //TODO: Check if there is apannelty for creating this "scope" again and again
            using (var scope = Microsoft.Bot.Builder.Dialogs.Conversation.Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IBotFrameworkInstrumentation>();
                await service.TrackActivity(activity, botData);
            }
        }
    }
}
