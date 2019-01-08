﻿namespace Bot.Ibex.Instrumentation.Tests.Instrumentations
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using Instrumentation.Instrumentations;
    using Instrumentation.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Schema;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Sentiments;
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

        [Theory(DisplayName = "GIVEN any activity WHEN TrackMessageSentiment is invoked THEN even telemetry is being sent")]
        [AutoMockData]
        public async void GivenAnyActivity_WhenTrackEventIsInvoked_ThenEvenTelemetryIsBeingSent(
            double sentimentScore,
            IMessageActivity activity,
            ISentimentClient sentimentClient,
            Settings settings)
        {
            // Arrange
            var instrumentation = new SentimentInstrumentation(sentimentClient, this.telemetryClient, settings);
            Mock.Get(sentimentClient).Setup(s => s.GetSentiment(activity)).Returns(Task.FromResult<double?>(sentimentScore));

            // Act
            await instrumentation.TrackMessageSentiment(activity).ConfigureAwait(false);

            // Assert
            this.mockTelemetryChannel.Verify(tc => tc.Send(It.Is<EventTelemetry>(t =>
                t.Name == EventTypes.MessageSentiment &&
                t.Properties[SentimentConstants.Score] == sentimentScore.ToString(CultureInfo.InvariantCulture))));
        }

        [Theory(DisplayName = "GIVEN empty activity WHEN TrackMessageSentiment is invoked THEN exception is thrown")]
        [AutoMockData]
        public async void GivenEmptyActivity_WhenTrackMessageSentimentIsInvoked_ThenExceptionIsThrown(
            ISentimentClient sentimentClient,
            Settings settings)
        {
            // Arrange
            IMessageActivity activity = null;
            var instrumentation = new SentimentInstrumentation(sentimentClient, this.telemetryClient, settings);

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => instrumentation.TrackMessageSentiment(activity)).ConfigureAwait(false);
        }

        [Theory(DisplayName = "GIVEN empty sentiment client WHEN constructor is invoked THEN exception is thrown")]
        [AutoData]
        public void GivenEmptySentimentClient_WhenConstructorIsInvoked_ThenExceptionIsThrown(Settings settings)
        {
            // Arrange
            const ISentimentClient sentimentClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentation(sentimentClient, this.telemetryClient, settings));
        }

        [Theory(DisplayName = "GIVEN empty telemetry client WHEN constructor is invoked THEN exception is thrown")]
        [AutoMockData]
        public void GivenEmptyTelemetryClient_WhenConstructorIsInvoked_ThenExceptionIsThrown(ISentimentClient sentimentClient, Settings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentation(sentimentClient, emptyTelemetryClient, settings));
        }

        [Theory(DisplayName = "GIVEN empty settings WHEN constructor is invoked THEN exception is thrown")]
        [AutoMockData]
        public void GivenEmptySettings_WhenConstructorIsInvoked_ThenExceptionIsThrown(ISentimentClient sentimentClient)
        {
            // Arrange
            const Settings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentation(sentimentClient, this.telemetryClient, emptySettings));
        }
    }
}
