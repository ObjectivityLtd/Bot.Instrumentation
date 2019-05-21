namespace Bot.Ibex.Instrumentation.Common.Rest
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Rest;

    public class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        public const string SubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";

        public ApiKeyServiceClientCredentials(string subscriptionKey)
        {
            this.SubscriptionKey = subscriptionKey;
        }

        public string SubscriptionKey { get; }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }

            request.Headers.Add(SubscriptionKeyHeaderName, this.SubscriptionKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}
