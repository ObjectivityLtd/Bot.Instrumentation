namespace Objectivity.Bot.Ibex.Instrumentation.Common.Tests.Instrumentations
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using AutoFixture.XUnit2.AutoMoq.Attributes;
    using Common.Instrumentations;
    using Common.Sentiments;
    using Common.Telemetry;
    using Constants;
    using global::AutoFixture.Xunit2;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Moq;
    using Settings;
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
            var telemetryConfiguration =
                new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName =
            "GIVEN any activity WHEN TrackMessageSentiment is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public async void GivenAnyActivityWhenTrackEventIsInvokedThenEventTelemetryIsBeingSent(
            double sentimentScore,
            IActivity activity,
            ISentimentClient sentimentClient,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new SentimentInstrumentation();

            Mock.Get(sentimentClient)
               .Setup(s => s.GetSentiment(activity))
               .Returns(Task.FromResult<double?>(sentimentScore));

            // Act
            await instrumentation.TrackMessageSentiment(activity, this.telemetryClient, settings, sentimentClient)
                .ConfigureAwait(false);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.MessageSentiment &&
                    t.Properties[SentimentConstants.Score] == sentimentScore.ToString(CultureInfo.InvariantCulture))),
                Times.Once);
        }

        [Theory(DisplayName =
            "GIVEN empty activity WHEN TrackMessageSentiment is invoked THEN exception is being thrown")]
        [AutoMockData]
        public async void GivenEmptyActivityWhenTrackMessageSentimentIsInvokedThenExceptionIsBeingThrown(
            ISentimentClient sentimentClient,
            InstrumentationSettings settings)
        {
            // Arrange
            const IActivity emptyActivity = null;
            var instrumentation = new SentimentInstrumentation();

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => instrumentation.TrackMessageSentiment(emptyActivity, this.telemetryClient, settings, sentimentClient))
                .ConfigureAwait(false);
        }

        //[Theory(DisplayName =
        //    "GIVEN empty sentiment client WHEN SentimentInstrumentation is constructed THEN exception is being thrown")]
        //[AutoData]
        //public void GivenEmptySentimentClientWhenSentimentInstrumentationIsConstructedThenExceptionIsBeingThrown(
        //    InstrumentationSettings settings)
        //{
        //    // Arrange
        //    const ISentimentClient emptySentimentClient = null;

        //    // Act
        //    // Assert
        //    Assert.Throws<ArgumentNullException>(() =>
        //        new SentimentInstrumentation(settings, this.telemetryClient, emptySentimentClient));
        //}

        //[Theory(DisplayName =
        //    "GIVEN empty telemetry client WHEN SentimentInstrumentation is constructed THEN exception is being thrown")]
        //[AutoMockData]
        //public void GivenEmptyTelemetryClientWhenSentimentInstrumentationIsConstructedThenExceptionIsBeingThrown(
        //    ISentimentClient sentimentClient,
        //    InstrumentationSettings settings)
        //{
        //    // Arrange
        //    const TelemetryClient emptyTelemetryClient = null;

        //    // Act
        //    // Assert
        //    Assert.Throws<ArgumentNullException>(() =>
        //        new SentimentInstrumentation(settings, emptyTelemetryClient, sentimentClient));
        //}

        //[Theory(DisplayName =
        //    "GIVEN empty settings WHEN SentimentInstrumentation is constructed THEN exception is being thrown")]
        //[AutoMockData]
        //public void GivenEmptySettingsWhenSentimentInstrumentationIsConstructedThenExceptionIsBeingThrown(
        //    ISentimentClient sentimentClient)
        //{
        //    // Arrange
        //    const InstrumentationSettings emptySettings = null;

        //    // Act
        //    // Assert
        //    Assert.Throws<ArgumentNullException>(() =>
        //        new SentimentInstrumentation(emptySettings, this.telemetryClient, sentimentClient));
        //}
    }
}
