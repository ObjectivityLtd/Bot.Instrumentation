﻿namespace Bot.Instrumentation.Common.Sentiments
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Bot.Instrumentation.Common.Adapters;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Rest;
    using Bot.Instrumentation.Common.Settings;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

    public class SentimentClient : ISentimentClient
    {
        private readonly ITextAnalyticsClient textAnalyticsClient;
        private bool disposed = false;

        public SentimentClient(SentimentClientSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.textAnalyticsClient = new TextAnalyticsClient(new ApiKeyServiceClientCredentials(settings.ApiSubscriptionKey))
            {
                Endpoint = settings.Endpoint,
            };
        }

        public SentimentClient(ITextAnalyticsClient textAnalyticsClient)
        {
            this.textAnalyticsClient = textAnalyticsClient ?? throw new ArgumentNullException(nameof(textAnalyticsClient));
        }

        public async Task<double?> GetSentiment(IActivityAdapter activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            MultiLanguageBatchInput input = activity.ToSentimentInput();
            SentimentBatchResult result = await this.textAnalyticsClient.SentimentAsync(null, input)
                                            .ConfigureAwait(false);

            return result?.Documents[0].Score;
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
                    this.textAnalyticsClient.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
