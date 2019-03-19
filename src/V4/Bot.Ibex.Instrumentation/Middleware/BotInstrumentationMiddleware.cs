namespace Bot.Ibex.Instrumentation.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Bot.Ibex.Instrumentation.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public class BotInstrumentationMiddleware : IMiddleware
    {
        private readonly TelemetryClient telemetryClient;
        private readonly InstrumentationSettings settings;

        public BotInstrumentationMiddleware(TelemetryClient telemetryClient, InstrumentationSettings settings)
        {
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task OnTurnAsync(
            ITurnContext turnContext,
            NextDelegate next,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BotAssert.ContextNotNull(turnContext);

            if (turnContext.Activity != null)
            {
                var et = this.BuildEventTelemetry(turnContext.Activity);
                this.telemetryClient.TrackEvent(et);
            }

            // hook up onSend pipeline
            turnContext.OnSendActivities(async (ctx, activities, nextSend) =>
            {
                activities.ForEach(a => this.telemetryClient.TrackEvent(this.BuildEventTelemetry(a)));

                return await nextSend()
                    .ConfigureAwait(false);
            });

            // hook up update activity pipeline
            turnContext.OnUpdateActivity(async (ctx, activity, nextUpdate) =>
            {
                var et = this.BuildEventTelemetry(activity);
                this.telemetryClient.TrackEvent(et);

                return await nextUpdate()
                    .ConfigureAwait(false);
            });

            if (next != null)
            {
                await next(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private EventTelemetry BuildEventTelemetry(IActivity activity)
        {
            var builder = new EventTelemetryBuilder(activity, this.settings);
            return builder.Build();
        }
    }
}