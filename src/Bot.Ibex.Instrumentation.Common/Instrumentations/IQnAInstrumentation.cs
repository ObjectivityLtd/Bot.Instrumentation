namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Microsoft.ApplicationInsights;

    public interface IQnAInstrumentation
    {
        void TrackEvent(IActivityAdapter activity, QueryResult queryResult, InstrumentationSettings settings, TelemetryClient telemetryClient);
    }
}
