﻿namespace Bot.Instrumentation.Common.Tests.Sentiments
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Bot.Instrumentation.Common.Adapters;
    using Bot.Instrumentation.Common.Sentiments;
    using Bot.Instrumentation.Common.Settings;
    using FluentAssertions;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
    using Microsoft.Rest;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("SentimentClient")]
    [Trait("Category", "Sentiments")]
    public class SentimentClientTests
    {
        [Theory(DisplayName = "GIVEN any message activity WHEN GetSentiment is invoked THEN score is being returned")]
        [AutoMockData]
        public async void GIVENAnyMessageActivity_WHENGetSentimentIsInvoked_THENScoreIsBeingReturned(
            ITextAnalyticsClient textAnalyticsClient,
            IActivityAdapter activity,
            double sentiment)
        {
            // Arrange
            var instrumentation = new SentimentClient(textAnalyticsClient);
            var response = new HttpOperationResponse<SentimentBatchResult>
            {
                Body = new SentimentBatchResult(new[] { new SentimentBatchResultItem(null, sentiment) }),
            };
            Mock.Get(textAnalyticsClient)
                .Setup(tac => tac.SentimentWithHttpMessagesAsync(
                    It.IsAny<bool?>(),
                    It.IsAny<MultiLanguageBatchInput>(),
                    It.IsAny<Dictionary<string, List<string>>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(response));

            // Act
            var score = await instrumentation.GetSentiment(activity)
                .ConfigureAwait(false);

            // Assert
            score.Should().Be(sentiment);
        }

        [Theory(DisplayName =
            "GIVEN empty message activity WHEN GetSentiment is invoked THEN exception is being thrown")]
        [AutoMockData]
        public async void GIVENEmptyMessageActivity_WHENGetSentimentIsInvoked_THENExceptionIsBeingThrown(
            ITextAnalyticsClient textAnalyticsClient)
        {
            // Arrange
            var instrumentation = new SentimentClient(textAnalyticsClient);
            const IActivityAdapter emptyMessageActivity = null;

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => instrumentation.GetSentiment(emptyMessageActivity))
                .ConfigureAwait(false);
        }

        [Theory(DisplayName =
            "GIVEN disposed SentimentClient WHEN GetSentiment is invoked THEN exception is being thrown")]
        [AutoMockData]
        public async void GIVENDisposedSentimentClient_WHENGetSentimentIsInvoked_THENExceptionIsBeingThrown(
            ITextAnalyticsClient textAnalyticsClient,
            IActivityAdapter activity)
        {
            // Arrange
            var instrumentation = new SentimentClient(textAnalyticsClient);
            instrumentation.Dispose();

            // Act
            // Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => instrumentation.GetSentiment(activity))
                .ConfigureAwait(false);
        }

        [Fact(DisplayName =
            "GIVEN empty sentiment client settings WHEN SentimentClient is created THEN exception is being thrown")]
        public void GIVENEmptySentimentClientSettings_WHENSentimentClientIsCreated_THENExceptionIsBeingThrown()
        {
            // Arrange
            const SentimentClientSettings emptySentimentClientSettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentClient(emptySentimentClientSettings));
        }

        [Fact(DisplayName =
            "GIVEN empty text analytics client WHEN SentimentClient is created THEN exception is being thrown")]
        public void GIVENEmptyTextAnalyticsClient_WHENSentimentClientIsCreated_THENExceptionIsBeingThrown()
        {
            // Arrange
            const ITextAnalyticsClient emptyTextAnalyticsClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentClient(emptyTextAnalyticsClient));
        }

        [Theory(DisplayName =
            "GIVEN SentimentClient WHEN Dispose is invoked THEN other resources are being disposed as well")]
        [AutoMockData]
        public void GIVENSentimentClient_WHENDisposeIsInvoked_THENOtherResourcesAreBeingDisposedAsWell(
            ITextAnalyticsClient textAnalyticsClient)
        {
            // Arrange
            var sentimentClient = new SentimentClient(textAnalyticsClient);

            // Act
            sentimentClient.Dispose();

            // Assert
            Mock.Get(textAnalyticsClient).Verify(sc => sc.Dispose(), Times.Once);
        }
    }
}
