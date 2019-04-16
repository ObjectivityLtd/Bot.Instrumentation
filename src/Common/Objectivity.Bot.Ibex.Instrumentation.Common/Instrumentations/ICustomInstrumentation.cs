namespace Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System.Collections.Generic;
    using Constants;
    using Telemetry;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}
