namespace Bot.Instrumentation.V4.Instrumentations
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;

    public interface ISentimentInstrumentation
    {
        Task TrackMessageSentiment(IMessageActivity activity);
    }
}
