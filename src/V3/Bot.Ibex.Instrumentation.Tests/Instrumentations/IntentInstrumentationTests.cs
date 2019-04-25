namespace Bot.Ibex.Instrumentation.V3.Tests.Instrumentations
{
    using System;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Common.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Moq;
    using Newtonsoft.Json;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using V3.Instrumentations;
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
            var telemetryConfiguration =
                new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackIntent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GIVENAnyActivity_WHENTrackIntentIsInvoked_THENEventTelemetryIsBeingSent(
            IMessageActivity activity,
            LuisResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation(this.telemetryClient, settings);

            // Act
            instrumentation.TrackIntent(activity, luisResult);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.Intent &&
                    t.Properties[IntentConstants.Intent] == luisResult.TopScoringIntent.Intent &&
                    t.Properties[IntentConstants.Score] == luisResult.TopScoringIntent.Score.ToString() &&
                    t.Properties[IntentConstants.Entities] == JsonConvert.SerializeObject(luisResult.Entities))),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackIntent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GIVENEmptyActivity_WHENTrackIntentIsInvoked_THENExceptionIsBeingThrown(
            LuisResult luisResult,
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
        public void GIVENEmptyQueryResult_WHENTrackIntentIsInvoked_THENExceptionIsBeingThrown(
            IMessageActivity activity,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation(this.telemetryClient, settings);
            const LuisResult emptyLuisResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackIntent(activity, emptyLuisResult));
        }

        [Theory(DisplayName =
            "GIVEN empty telemetry client WHEN IntentInstrumentation is constructed THEN exception is being thrown")]
        [AutoData]
        public void GIVENEmptyTelemetryClient_WHENIntentInstrumentationIsConstructed_THENExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new IntentInstrumentation(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName =
            "GIVEN empty settings WHEN IntentInstrumentation is constructed THEN exception is being thrown")]
        public void GIVENEmptySettings_WHENIntentInstrumentationIsConstructed_THENExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new IntentInstrumentation(this.telemetryClient, emptySettings));
        }
    }
}