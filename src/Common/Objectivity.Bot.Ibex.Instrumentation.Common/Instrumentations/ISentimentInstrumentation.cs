namespace Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Threading.Tasks;
    using Telemetry;

    public interface ISentimentInstrumentation
    {
        Task TrackMessageSentiment(IActivity activity);
    }
}
