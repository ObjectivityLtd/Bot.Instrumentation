namespace Bot.Ibex.Instrumentation.Common.Tests.Instrumentations
{
    using System;
    using Common.Instrumentations;
    using Common.Telemetry;
    using global::AutoFixture.Xunit2;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Models;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
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
        public void GIVENAnyActivity_WHENTrackIntentIsInvoked_THENEventTelemetryIsBeingSent(
            IActivityAdapter activity,
            IntentResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation();

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
            GIVENAnyActivityAnyResultAnAnyProperty_WHENTrackIntentIsInvoked_THENEventTelemetryIsBeingSent(
                IActivityAdapter activity,
                IntentResult result,
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
        public void GIVENEmptyActivity_WHENTrackIntentIsInvoked_THENExceptionIsBeingThrown(
            IntentResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation();
            const IActivityAdapter emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackIntent(emptyActivity, luisResult, this.telemetryClient, settings));
        }
    }
}
