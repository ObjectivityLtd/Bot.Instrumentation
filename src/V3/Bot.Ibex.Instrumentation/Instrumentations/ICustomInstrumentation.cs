namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Constants;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}
