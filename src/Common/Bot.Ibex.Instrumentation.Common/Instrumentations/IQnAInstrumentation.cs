namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using Microsoft.ApplicationInsights;
    using Models;
    using Settings;
    using Telemetry;

    public interface IQnAInstrumentation
    {
        void TrackEvent(IActivityAdapter activity, QueryResult queryResult, InstrumentationSettings settings, TelemetryClient telemetryClient);
    }
}
