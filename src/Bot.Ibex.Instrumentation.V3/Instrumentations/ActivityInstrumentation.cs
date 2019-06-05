namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Threading.Tasks;
    using Bot.Ibex.Instrumentation.Common.Extensions;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Bot.Ibex.Instrumentation.V3.Adapters;
    using Microsoft.ApplicationInsights;

    public class ActivityInstrumentation : IActivityInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public ActivityInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public Task TrackActivity(Microsoft.Bot.Connector.IActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            return Task.Run(() =>
            {
                var activityAdapter = new ActivityAdapter(activity);
                activityAdapter.TrackCustomEvent(this.telemetryClient, this.settings, EventTypes.ActivityEvent);
            });
        }
    }
}
