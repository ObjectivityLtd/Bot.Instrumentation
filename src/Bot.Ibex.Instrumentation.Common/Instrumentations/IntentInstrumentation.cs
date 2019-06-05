namespace Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Microsoft.ApplicationInsights;

    public class IntentInstrumentation : IIntentInstrumentation
    {
        public void TrackIntent(IActivityAdapter activity, IntentResult result, TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (telemetryClient is null)
            {
                throw new ArgumentNullException(nameof(telemetryClient));
            }

            var properties = new Dictionary<string, string>
            {
                { IntentConstants.Intent, result.Intent },
                { IntentConstants.Score, result.Score },
                { IntentConstants.Entities, result.Entities },
            };

            var instrumentation = new CustomInstrumentation();
            instrumentation.TrackCustomEvent(activity, telemetryClient, settings, EventTypes.Intent, properties);
        }
    }
}
