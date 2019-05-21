namespace Bot.Ibex.Instrumentation.Common.Sentiments
{
    using System;   
    using System.Threading.Tasks;
    using Telemetry;

    public interface ISentimentClient : IDisposable
    {
        Task<double?> GetSentiment(IActivityAdapter activity);
    }
}
