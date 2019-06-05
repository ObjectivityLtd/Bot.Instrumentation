namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Threading.Tasks;
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Sentiments;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Microsoft.ApplicationInsights;

    public interface ISentimentInstrumentation
    {
        Task TrackMessageSentiment(IActivityAdapter activity, TelemetryClient telemetryClient, InstrumentationSettings settings, ISentimentClient sentimentClient);
    }
}
