namespace Bot.Ibex.Instrumentation.V3.Sentiments
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    public interface ISentimentClient : IDisposable
    {
        Task<double?> GetSentiment(IActivity activity);
    }
}
