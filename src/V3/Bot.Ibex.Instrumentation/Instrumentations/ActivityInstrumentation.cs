namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Telemetry;

    [Serializable]
    public class ActivityInstrumentation : IActivityInstrumentation
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public ActivityInstrumentation(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public Task TrackActivity(IActivity activity)
        {
            return Task.Run(() =>
            {
                var builder = new EventTelemetryBuilder(activity, this.settings);
                var eventTelemetry = builder.Build();
                this.telemetryClient.TrackEvent(eventTelemetry);
            });
        }
    }
}
