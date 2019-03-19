namespace Bot.Ibex.Instrumentation.Tests.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Instrumentations;
    using Instrumentation.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Schema;
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
            var telemetryConfiguration = new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackCustomEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GivenAnyActivity_WhenTrackCustomEventIsInvoked_ThenEventTelemetryIsBeingSent(
            IMessageActivity activity,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new CustomInstrumentation(this.telemetryClient, settings);

            // Act
            instrumentation.TrackCustomEvent(activity);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.CustomEvent)),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN any activity, any event name and any property WHEN TrackCustomEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GivenAnyActivityAnyEventNameAndAnyProperty_WhenTrackCustomEventIsInvoked_ThenEventTelemetryIsBeingSent(
            IMessageActivity activity,
            string eventName,
            string propertyKey,
            string propertyValue,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new CustomInstrumentation(this.telemetryClient, settings);
            var properties = new Dictionary<string, string> { { propertyKey, propertyValue } };

            // Act
            instrumentation.TrackCustomEvent(activity, eventName, properties);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == eventName &&
                    t.Properties[propertyKey] == propertyValue)),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackCustomEvent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyActivity_WhenTrackCustomEventIsInvoked_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new CustomInstrumentation(this.telemetryClient, settings);
            const IActivity emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackCustomEvent(emptyActivity));
        }

        [Theory(DisplayName = "GIVEN empty telemetry client WHEN CustomInstrumentation is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyTelemetryClient_WhenCustomInstrumentationIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new CustomInstrumentation(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName = "GIVEN empty settings WHEN CustomInstrumentation is constructed THEN exception is being thrown")]
        public void GivenEmptySettings_WhenCustomInstrumentationIsConstructed_ThenExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new CustomInstrumentation(this.telemetryClient, emptySettings));
        }
    }
}
