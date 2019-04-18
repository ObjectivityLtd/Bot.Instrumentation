namespace Objectivity.Bot.Ibex.Instrumentation.Common.Tests.Instrumentations
{
    using System;
    using System.Globalization;
    using AutoFixture.XUnit2.AutoMoq.Attributes;
    using Common.Instrumentations;
    using Common.Telemetry;
    using Constants;
    using global::AutoFixture.Xunit2;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Models;
    using Moq;
    using Settings;
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
            IActivity activity,
            RecognizedIntentResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation();
            //var topScoringIntent = luisResult.GetTopScoringIntent();

            // Act
            instrumentation.TrackIntent(activity, luisResult, this.telemetryClient, settings);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.Intent &&
                    t.Properties[IntentConstants.Intent] == luisResult.Intent &&
                    t.Properties[IntentConstants.Score] == luisResult.Score &&
                    t.Properties[IntentConstants.Entities] == luisResult.Entities)),
                Times.Once);
        }

        [Theory(DisplayName =
            "GIVEN any activity, any result and any property WHEN TrackIntent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void
            GivenAnyActivityAnyResultAnAnyPropertyWhenTrackIntentIsInvokedThenEventTelemetryIsBeingSent(
                IActivity activity,
                RecognizedIntentResult result,
                InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation();

            // Act
            instrumentation.TrackIntent(activity, result, this.telemetryClient, settings);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.Intent &&
                    t.Properties[IntentConstants.Intent] == result.Intent &&
                    t.Properties[IntentConstants.Score] == result.Score &&
                    t.Properties[IntentConstants.Entities] == result.Entities)),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackIntent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyActivity_WhenTrackIntentIsInvoked_ThenExceptionIsBeingThrown(
            RecognizedIntentResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation();
            const IActivity emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackIntent(emptyActivity, luisResult, this.telemetryClient, settings));
        }
    }
}
