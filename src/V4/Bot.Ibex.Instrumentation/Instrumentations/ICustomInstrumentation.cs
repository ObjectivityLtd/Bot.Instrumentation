namespace Bot.Ibex.Instrumentation.Instrumentations
{
    using System.Collections.Generic;
    using Bot.Ibex.Instrumentation.Telemetry;
    using Microsoft.Bot.Schema;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}