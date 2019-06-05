namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Microsoft.ApplicationInsights;

    public interface IIntentInstrumentation
    {
        void TrackIntent(IActivityAdapter activity, IntentResult result, TelemetryClient telemetryClient, InstrumentationSettings settings);
    }
}
