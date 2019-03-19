namespace Bot.Ibex.Instrumentation.Sentiments
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Schema;

    public interface ISentimentClient : IDisposable
    {
        Task<double?> GetSentiment(IMessageActivity activity);
    }
}