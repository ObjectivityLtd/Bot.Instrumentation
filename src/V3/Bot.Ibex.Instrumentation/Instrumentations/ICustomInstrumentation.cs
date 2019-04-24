namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System.Collections.Generic;
    using Bot.Ibex.Instrumentation.Common.Constants;
    using Microsoft.Bot.Connector;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}
