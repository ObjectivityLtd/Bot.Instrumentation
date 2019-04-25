namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Threading.Tasks;
    using Adapters;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
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
                var objectivityActivity = new ActivityAdapter(activity);
                var builder = new EventTelemetryBuilder(objectivityActivity, this.settings);
                var eventTelemetry = builder.Build();
                eventTelemetry.Name = EventTypes.ActivityEvent;
                this.telemetryClient.TrackEvent(eventTelemetry);
            });
        }
    }
}
