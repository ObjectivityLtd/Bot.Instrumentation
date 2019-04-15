namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Constants;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            Microsoft.Bot.Schema.IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}