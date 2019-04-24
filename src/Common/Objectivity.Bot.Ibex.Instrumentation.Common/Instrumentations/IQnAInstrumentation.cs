namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using Microsoft.ApplicationInsights;
    using Settings;
    using Telemetry;

    public interface IQnAInstrumentation
    {
        void TrackEvent(IActivity activity, QueryResult queryResult, InstrumentationSettings settings, TelemetryClient telemetryClient);
    }
}
