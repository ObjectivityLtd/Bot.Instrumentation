namespace Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using Microsoft.ApplicationInsights;
    using Models;
    using Settings;
    using Telemetry;

    public interface IIntentInstrumentation
    {
        void TrackIntent(IActivity activity, RecognizedIntentResult result, TelemetryClient telemetryClient, InstrumentationSettings settings);
    }
}
