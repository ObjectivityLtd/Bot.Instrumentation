namespace Bot.Instrumentation.Common.Sentiments
{
    using System;
    using System.Threading.Tasks;
    using Bot.Instrumentation.Common.Adapters;

    public interface ISentimentClient : IDisposable
    {
        Task<double?> GetSentiment(IActivityAdapter activity);
    }
}
