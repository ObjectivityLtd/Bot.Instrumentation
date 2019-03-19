namespace Bot.Ibex.Instrumentation.Tests.Middleware
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Middleware;
    using Bot.Ibex.Instrumentation.Sentiments;
    using FluentAssertions;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("SentimentInstrumentationMiddleware")]
    [Trait("Category", "Middleware")]
    public class SentimentInstrumentationMiddlewareTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public SentimentInstrumentationMiddlewareTests()
        {
            var telemetryConfiguration = new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN turn context with any activity WHEN OnTurnAsync is invoked THEN sentiment retrieved event telemetry is being sent")]
        [InlineAutoMockData(ActivityTypes.Message, 1)]
        [InlineAutoMockData(ActivityTypes.MessageUpdate, 0)]
        public async void GivenTurnContextWithAnyActivity_WhenOnTurnAsyncIsInvoked_ThenSentimentRetrievedEventTelemetryIsBeingSent(
            string activityType,
            int expectedNumberOfInvocations,
            Activity activity,
            ITurnContext turnContext,
            ISentimentClient sentimentClient,
            InstrumentationSettings settings)
        {
            // Arrange
            activity.Type = activityType;
            activity.ReplyToId = null;
            Mock.Get(turnContext)
                .SetupGet(c => c.Activity)
                .Returns(activity);
            var middleware = new SentimentInstrumentationMiddleware(this.telemetryClient, sentimentClient, settings);

            // Act
            await middleware.OnTurnAsync(turnContext, null)
                .ConfigureAwait(false);

            // Assert
            Mock.Get(sentimentClient).Verify(sc => sc.GetSentiment(It.IsAny<IMessageActivity>()), Times.Exactly(expectedNumberOfInvocations));
            this.mockTelemetryChannel.Verify(tc => tc.Send(It.IsAny<EventTelemetry>()), Times.Exactly(expectedNumberOfInvocations));
        }

        [Theory(DisplayName = "GIVEN next turn WHEN OnTurnAsync is invoked THEN next turn is being invoked")]
        [AutoMockData]
        public async void GivenNextTurn_WhenOnTurnAsyncIsInvoked_ThenNextTurnIsBeingInvoked(
            ITurnContext turnContext,
            SentimentInstrumentationMiddlewareSettings settings)
        {
            // Arrange
            var instrumentation = new SentimentInstrumentationMiddleware(this.telemetryClient, settings);
            var nextTurnInvoked = false;

            // Act
            await instrumentation.OnTurnAsync(turnContext, token => Task.Run(() => nextTurnInvoked = true, token))
                .ConfigureAwait(false);

            // Assert
            nextTurnInvoked.Should().Be(true);
        }

        [Theory(DisplayName = "GIVEN disposed SentimentInstrumentationMiddleware WHEN OnTurnAsync is invoked THEN exception is being thrown")]
        [AutoData]
        public async void GivenDisposedSentimentInstrumentationMiddleware_WhenOnTurnAsyncIsInvoked_ThenExceptionIsBeingThrown(
            SentimentInstrumentationMiddlewareSettings settings)
        {
            // Arrange
            var instrumentation = new SentimentInstrumentationMiddleware(this.telemetryClient, settings);
            instrumentation.Dispose();

            // Act
            // Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => instrumentation.OnTurnAsync(null, null))
                .ConfigureAwait(false);
        }

        [Theory(DisplayName = "GIVEN empty turn context WHEN OnTurnAsync is invoked THEN exception is being thrown")]
        [AutoData]
        public async void GivenEmptyTurnContext_WhenOnTurnAsyncIsInvoked_ThenExceptionIsBeingThrown(
            SentimentInstrumentationMiddlewareSettings settings)
        {
            // Arrange
            var instrumentation = new SentimentInstrumentationMiddleware(this.telemetryClient, settings);
            const ITurnContext emptyTurnContext = null;
            NextDelegate nextDelegate = Task.FromCanceled;

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => instrumentation.OnTurnAsync(emptyTurnContext, nextDelegate))
                .ConfigureAwait(false);
        }

        [Theory(DisplayName = "GIVEN empty telemetry client and any settings WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyTelemetryClientAndAnySettings_WhenSentimentInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown(
            SentimentInstrumentationMiddlewareSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddleware(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName = "GIVEN any telemetry client and empty settings WHEN BotInstrumentationMiddleware is constructed THEN exception is being thrown")]
        public void GivenAnyTelemetryClientAndEmptySettings_WhenBotInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown()
        {
            // Arrange
            const SentimentInstrumentationMiddlewareSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddleware(this.telemetryClient, emptySettings));
        }

        [Theory(DisplayName = "GIVEN empty telemetry client, any settings and any text analytics client WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptyTelemetryClientAnySettingsAndAnyTextAnalyticsClient_WhenSentimentInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings,
            ITextAnalyticsClient textAnalyticsClient)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddleware(emptyTelemetryClient, textAnalyticsClient, settings));
        }

        [Theory(DisplayName = "GIVEN any telemetry client, any settings and any text analytics client WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenAnyTelemetryClientEmptySettingsAndAnyTextAnalyticsClient_WhenSentimentInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown(
            ITextAnalyticsClient textAnalyticsClient)
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddleware(this.telemetryClient, textAnalyticsClient, emptySettings));
        }

        [Theory(DisplayName = "GIVEN any telemetry client, any settings and empty text analytics client WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenAnyTelemetryClientAnySettingsAndEmptyTextAnalyticsClient_WhenSentimentInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const ITextAnalyticsClient emptyTextAnalyticsClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddleware(this.telemetryClient, emptyTextAnalyticsClient, settings));
        }

        [Theory(DisplayName = "GIVEN empty telemetry client, any settings and any sentiment client WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptyTelemetryClientAnySettingsAndAnySentimentClient_WhenSentimentInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings,
            ISentimentClient sentimentClient)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddleware(emptyTelemetryClient, sentimentClient, settings));
        }

        [Theory(DisplayName = "GIVEN any telemetry client, empty settings and any sentiment client WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenAnyTelemetryClientEmptySettingsAndAnySentimentClient_WhenSentimentInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown(
            ISentimentClient sentimentClient)
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddleware(this.telemetryClient, sentimentClient, emptySettings));
        }

        [Theory(DisplayName = "GIVEN any telemetry client, any settings and empty sentiment client WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenAnyTelemetryClientAnySettingsAndEmptySentimentClient_WhenSentimentInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const ISentimentClient emptySentimentClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddleware(this.telemetryClient, emptySentimentClient, settings));
        }

        [Theory(DisplayName = "GIVEN SentimentInstrumentationMiddleware WHEN Dispose is invoked THEN other resources are being disposed as well")]
        [AutoMockData]
        public void GivenSentimentInstrumentationMiddleware_WhenDisposeIsInvoked_ThenOtherResourcesAreBeingDisposedAsWell(
            InstrumentationSettings settings,
            ISentimentClient sentimentClient)
        {
            // Arrange
            var instrumentation = new SentimentInstrumentationMiddleware(this.telemetryClient, sentimentClient, settings);

            // Act
            instrumentation.Dispose();

            // Assert
            Mock.Get(sentimentClient).Verify(sc => sc.Dispose(), Times.Once);
        }
    }
}
