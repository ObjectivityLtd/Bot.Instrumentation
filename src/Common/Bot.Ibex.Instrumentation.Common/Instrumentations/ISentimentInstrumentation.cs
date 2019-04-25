namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Sentiments;
    using Settings;
    using Telemetry;

    public interface ISentimentInstrumentation
    {
        Task TrackMessageSentiment(IActivityAdapter activity, TelemetryClient telemetryClient, InstrumentationSettings settings, ISentimentClient sentimentClient);
    }
}
