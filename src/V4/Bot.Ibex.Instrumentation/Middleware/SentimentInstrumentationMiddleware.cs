namespace Bot.Ibex.Instrumentation.V4.Middleware
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Adapters;
    using Bot.Ibex.Instrumentation.Common.Extensions;
    using Bot.Ibex.Instrumentation.Common.Sentiments;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Instrumentations;
    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.Bot.Builder;

    public class SentimentInstrumentationMiddleware : IMiddleware, IDisposable
    {
        private readonly ISentimentClient sentimentClient;
        private readonly ISentimentInstrumentation sentimentInstrumentation;
        private bool disposed = false;

        public SentimentInstrumentationMiddleware(
            TelemetryClient telemetryClient,
            SentimentInstrumentationMiddlewareSettings middlewareSettings)
            : this(
                telemetryClient,
                new SentimentClient(middlewareSettings?.SentimentClientSettings),
                middlewareSettings?.InstrumentationSettings)
        {
        }

        public SentimentInstrumentationMiddleware(
            TelemetryClient telemetryClient,
            ITextAnalyticsClient textAnalyticsClient,
            InstrumentationSettings instrumentationSettings)
            : this(
                telemetryClient,
                new SentimentClient(textAnalyticsClient),
                instrumentationSettings)
        {
        }

        public SentimentInstrumentationMiddleware(
            TelemetryClient telemetryClient,
            ISentimentClient sentimentClient,
            InstrumentationSettings instrumentationSettings)
        {
            this.sentimentClient = sentimentClient ?? throw new ArgumentNullException(nameof(sentimentClient));
            this.sentimentInstrumentation = new SentimentInstrumentation(this.sentimentClient, telemetryClient, instrumentationSettings);
        }

        public async Task OnTurnAsync(
            ITurnContext turnContext,
            NextDelegate next,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            BotAssert.ContextNotNull(turnContext);

            var objectivityActivity = new TurnContextAdapter(turnContext);
            if (objectivityActivity.IsIncomingMessage())
            {
                await this.sentimentInstrumentation.TrackMessageSentiment(turnContext.Activity)
                    .ConfigureAwait(false);
            }

            if (next != null)
            {
                await next(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "No need for finalizer on menaged resources.")]
        [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Correct implementation without finalizer.")]
        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.sentimentClient.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
