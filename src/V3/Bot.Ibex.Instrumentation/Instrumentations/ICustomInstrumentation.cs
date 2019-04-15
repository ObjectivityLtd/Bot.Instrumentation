namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System.Collections.Generic;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Constants;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using IActivity = Microsoft.Bot.Connector.IActivity;

    public interface ICustomInstrumentation
    {
        void TrackCustomEvent(
            IActivity activity,
            string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> properties = null);
    }
}
