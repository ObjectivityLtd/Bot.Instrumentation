namespace Bot.Ibex.Instrumentation.V3.Tests.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Bot.Ibex.Instrumentation.V3.Instrumentations;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Connector;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;
    using IActivity = Microsoft.Bot.Connector.IActivity;

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
        public void GIVENAnyActivity_WHENTrackCustomEventIsInvoked_THENEventTelemetryIsBeingSent(
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
        public void GIVENAnyActivityAnyEventNameAndAnyProperty_WHENTrackCustomEventIsInvoked_THENEventTelemetryIsBeingSent(
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
        public void GIVENEmptyActivity_WHENTrackCustomEventIsInvoked_THENExceptionIsBeingThrown(
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
        public void GIVENEmptyTelemetryClient_WHENCustomInstrumentationIsConstructed_THENExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new CustomInstrumentation(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName = "GIVEN empty settings WHEN CustomInstrumentation is constructed THEN exception is being thrown")]
        public void GIVENEmptySettings_WHENCustomInstrumentationIsConstructed_THENExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new CustomInstrumentation(this.telemetryClient, emptySettings));
        }
    }
}
