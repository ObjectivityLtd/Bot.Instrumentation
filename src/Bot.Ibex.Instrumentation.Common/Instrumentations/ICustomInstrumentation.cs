namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Collections.Generic;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Microsoft.ApplicationInsights;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            IActivityAdapter activity,
            TelemetryClient telemetryClient,
            InstrumentationSettings settings,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}
