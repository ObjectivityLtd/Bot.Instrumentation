namespace Bot.Ibex.Instrumentation.Common.Tests.Rest
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using Common.Rest;
    using FluentAssertions;
    using global::AutoFixture.Xunit2;
    using Xunit;

    [Collection("ApiKeyServiceClientCredentials")]
    [Trait("Category", "Rest")]
    public class ApiKeyServiceClientCredentialsTests
    {
        [Theory(DisplayName = "GIVEN ApiKeyServiceClientCredentials WHEN ProcessHttpRequestAsync is invoked THEN request headers decorated with subscription key")]
        [AutoData]
        public async void GivenApiKeyServiceClientCredentials_WhenProcessHttpRequestAsyncIsInvoked_ThenRequestHeadersDecoratedWithSubscriptionKey(
            ApiKeyServiceClientCredentials credentials)
        {
            // Arrange
            var request = new HttpRequestMessage();

            // Act
            await credentials.ProcessHttpRequestAsync(request, default(CancellationToken))
                .ConfigureAwait(false);

            // Assert
            var collection = request.Headers.ToList();
            collection.Should()
                .ContainSingle(p =>
                    p.Key == ApiKeyServiceClientCredentials.SubscriptionKeyHeaderName &&
                    p.Value.First() == credentials.SubscriptionKey);
        }

        [Theory(DisplayName = "GIVEN empty HttpRequestMessage WHEN ProcessHttpRequestAsync is invoked THEN exception is being thrown")]
        [AutoData]
        public async void GivenEmptyHttpRequestMessage_WhenProcessHttpRequestAsyncIsInvoked_ThenExceptionIsBeingThrown(
            ApiKeyServiceClientCredentials credentials)
        {
            // Arrange
            const HttpRequestMessage emptyHttpRequestMessage = null;

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => credentials.ProcessHttpRequestAsync(emptyHttpRequestMessage, default(CancellationToken)))
                .ConfigureAwait(false);
        }
    }
}
