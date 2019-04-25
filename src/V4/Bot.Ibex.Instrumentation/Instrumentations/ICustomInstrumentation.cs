namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System.Collections.Generic;
    using Common.Telemetry;
    using IActivity = Microsoft.Bot.Schema.IActivity;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}
