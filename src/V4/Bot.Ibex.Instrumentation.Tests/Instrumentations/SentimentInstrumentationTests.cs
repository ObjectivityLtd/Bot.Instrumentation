namespace Bot.Ibex.Instrumentation.Tests.Instrumentations
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Instrumentations;
    using Bot.Ibex.Instrumentation.Sentiments;
    using Bot.Ibex.Instrumentation.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Schema;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("SentimentInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class SentimentInstrumentationTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public SentimentInstrumentationTests()
        {
            var telemetryConfiguration = new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackMessageSentiment is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public async void GivenAnyActivity_WhenTrackEventIsInvoked_ThenEventTelemetryIsBeingSent(
            double sentimentScore,
            IMessageActivity activity,
            ISentimentClient sentimentClient,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new SentimentInstrumentation(sentimentClient, this.telemetryClient, settings);
            Mock.Get(sentimentClient)
                .Setup(s => s.GetSentiment(activity))
                .Returns(Task.FromResult<double?>(sentimentScore));

            // Act
            await instrumentation.TrackMessageSentiment(activity)
                .ConfigureAwait(false);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.MessageSentiment &&
                    t.Properties[SentimentConstants.Score] == sentimentScore.ToString(CultureInfo.InvariantCulture))),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity WHEN TrackMessageSentiment is invoked THEN exception is being thrown")]
        [AutoMockData]
        public async void GivenEmptyActivity_WhenTrackMessageSentimentIsInvoked_ThenExceptionIsBeingThrown(
            ISentimentClient sentimentClient,
            InstrumentationSettings settings)
        {
            // Arrange
            const IMessageActivity emptyActivity = null;
            var instrumentation = new SentimentInstrumentation(sentimentClient, this.telemetryClient, settings);

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => instrumentation.TrackMessageSentiment(emptyActivity))
                .ConfigureAwait(false);
        }

        [Theory(DisplayName = "GIVEN empty sentiment client WHEN SentimentInstrumentation is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptySentimentClient_WhenSentimentInstrumentationIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const ISentimentClient emptySentimentClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentation(emptySentimentClient, this.telemetryClient, settings));
        }

        [Theory(DisplayName = "GIVEN empty telemetry client WHEN SentimentInstrumentation is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptyTelemetryClient_WhenSentimentInstrumentationIsConstructed_ThenExceptionIsBeingThrown(
            ISentimentClient sentimentClient,
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentation(sentimentClient, emptyTelemetryClient, settings));
        }

        [Theory(DisplayName = "GIVEN empty settings WHEN SentimentInstrumentation is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptySettings_WhenSentimentInstrumentationIsConstructed_ThenExceptionIsBeingThrown(
            ISentimentClient sentimentClient)
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentation(sentimentClient, this.telemetryClient, emptySettings));
        }
    }
}
