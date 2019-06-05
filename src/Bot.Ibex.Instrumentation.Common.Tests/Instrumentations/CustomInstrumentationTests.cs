namespace Bot.Ibex.Instrumentation.Common.Tests.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Instrumentations;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("CustomInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class CustomInstrumentationTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public CustomInstrumentationTests()
        {
            var telemetryConfiguration =
                new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackCustomEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GIVENAnyActivity_WHENTrackCustomEventIsInvoked_THENEventTelemetryIsBeingSent(
            IActivityAdapter activity,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new CustomInstrumentation();

            // Act
            instrumentation.TrackCustomEvent(activity, this.telemetryClient, settings);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.CustomEvent)),
                Times.Once);
        }

        [Theory(DisplayName =
            "GIVEN any activity, any event name and any property WHEN TrackCustomEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void
            GIVENAnyActivityAnyEventNameAndAnyProperty_WHENTrackCustomEventIsInvoked_THENEventTelemetryIsBeingSent(
                IActivityAdapter activity,
                string eventName,
                string propertyKey,
                string propertyValue,
                InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new CustomInstrumentation();
            var properties = new Dictionary<string, string> { { propertyKey, propertyValue } };

            // Act
            instrumentation.TrackCustomEvent(activity, this.telemetryClient, settings, eventName, properties);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == eventName &&
                    t.Properties[propertyKey] == propertyValue)),
                Times.Once);
        }

        [Theory(DisplayName =
            "GIVEN empty activity result WHEN TrackCustomEvent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GIVENEmptyActivity_WHENTrackCustomEventIsInvoked_THENExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new CustomInstrumentation();
            const IActivityAdapter emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackCustomEvent(emptyActivity, this.telemetryClient, settings));
        }
    }
}