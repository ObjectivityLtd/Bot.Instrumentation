namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using Microsoft.ApplicationInsights;
    using Models;
    using Settings;
    using Telemetry;

    public interface IIntentInstrumentation
    {
        void TrackIntent(IActivityAdapter activity, IntentResult result, TelemetryClient telemetryClient, InstrumentationSettings settings);
    }
}
