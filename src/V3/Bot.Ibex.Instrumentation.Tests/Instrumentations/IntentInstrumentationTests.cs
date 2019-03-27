namespace Bot.Ibex.Instrumentation.V3.Tests.Instrumentations
{
    using System;
    using AutoFixture.Xunit2;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Moq;
    using Newtonsoft.Json;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using V3.Instrumentations;
    using Xunit;

    [Collection("IntentInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class IntentInstrumentationTests
    {
        public IntentInstrumentationTests()
        {
            var telemetryConfiguration =
                new TelemetryConfiguration(FakeInstrumentationKey, mockTelemetryChannel.Object);
            telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        [Theory(DisplayName = "GIVEN any activity WHEN TrackIntent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GivenAnyActivity_WhenTrackIntentIsInvoked_ThenEventTelemetryIsBeingSent(
            IMessageActivity activity,
            LuisResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation(telemetryClient, settings);

            // Act
            instrumentation.TrackIntent(activity, luisResult);

            // Assert
            mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.Intent &&
                    t.Properties[IntentConstants.Intent] == luisResult.TopScoringIntent.Intent &&
                    t.Properties[IntentConstants.Score] == luisResult.TopScoringIntent.Score.ToString() &&
                    t.Properties[IntentConstants.Entities] == JsonConvert.SerializeObject(luisResult.Entities))),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackIntent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyActivity_WhenTrackIntentIsInvoked_ThenExceptionIsBeingThrown(
            LuisResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation(telemetryClient, settings);
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
            var instrumentation = new IntentInstrumentation(telemetryClient, settings);
            const LuisResult emptyLuisResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackIntent(activity, emptyLuisResult));
        }

        [Theory(DisplayName =
            "GIVEN empty telemetry client WHEN IntentInstrumentation is constructed THEN exception is being thrown")]
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

        [Fact(DisplayName =
            "GIVEN empty settings WHEN IntentInstrumentation is constructed THEN exception is being thrown")]
        public void GivenEmptySettings_WhenIntentInstrumentationIsConstructed_ThenExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new IntentInstrumentation(telemetryClient, emptySettings));
        }
    }
}