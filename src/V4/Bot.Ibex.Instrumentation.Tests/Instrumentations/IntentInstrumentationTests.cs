namespace Bot.Ibex.Instrumentation.Tests.Instrumentations
{
    using System;
    using System.Globalization;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Instrumentations;
    using Bot.Ibex.Instrumentation.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Moq;
    using Newtonsoft.Json;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("IntentInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class IntentInstrumentationTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public IntentInstrumentationTests()
        {
            var telemetryConfiguration = new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackIntent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GivenAnyActivity_WhenTrackIntentIsInvoked_ThenEventTelemetryIsBeingSent(
            IMessageActivity activity,
            RecognizerResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation(this.telemetryClient, settings);
            var topScoringIntent = luisResult.GetTopScoringIntent();

            // Act
            instrumentation.TrackIntent(activity, luisResult);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.Intent &&
                    t.Properties[IntentConstants.Intent] == topScoringIntent.intent &&
                    t.Properties[IntentConstants.Score] == topScoringIntent.score.ToString(CultureInfo.InvariantCulture) &&
                    t.Properties[IntentConstants.Entities] == luisResult.Entities.ToString(Formatting.None))),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackIntent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyActivity_WhenTrackIntentIsInvoked_ThenExceptionIsBeingThrown(
            RecognizerResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation(this.telemetryClient, settings);
            const IMessageActivity emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackIntent(emptyActivity, luisResult));
        }

        [Theory(DisplayName = "GIVEN empty query result WHEN TrackIntent is invoked THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptyQueryResult_WhenTrackIntentIsInvoked_ThenExceptionIsBeingThrown(
            IMessageActivity activity,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation(this.telemetryClient, settings);
            const RecognizerResult emptyLuisResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackIntent(activity, emptyLuisResult));
        }

        [Theory(DisplayName = "GIVEN empty telemetry client WHEN IntentInstrumentation is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyTelemetryClient_WhenIntentInstrumentationIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new IntentInstrumentation(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName = "GIVEN empty settings WHEN IntentInstrumentation is constructed THEN exception is being thrown")]
        public void GivenEmptySettings_WhenIntentInstrumentationIsConstructed_ThenExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new IntentInstrumentation(this.telemetryClient, emptySettings));
        }
    }
}