namespace Bot.Ibex.Instrumentation.Instrumentations
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;

    public interface ISentimentInstrumentation
    {
        Task TrackMessageSentiment(IMessageActivity activity);
    }
}