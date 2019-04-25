namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Settings;
    using Telemetry;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            IActivity activity,
            TelemetryClient telemetryClient,
            InstrumentationSettings settings,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}
