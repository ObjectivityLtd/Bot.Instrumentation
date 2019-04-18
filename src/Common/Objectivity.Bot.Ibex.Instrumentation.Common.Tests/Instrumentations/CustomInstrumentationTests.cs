namespace Objectivity.Bot.Ibex.Instrumentation.Common.Tests.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using AutoFixture.XUnit2.AutoMoq.Attributes;
    using Common.Instrumentations;
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
        public void GivenAnyActivityWhenTrackCustomEventIsInvokedThenEventTelemetryIsBeingSent(
            IActivity activity,
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
            GivenAnyActivityAnyEventNameAndAnyPropertyWhenTrackCustomEventIsInvokedThenEventTelemetryIsBeingSent(
                IActivity activity,
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
        public void GivenEmptyActivityWhenTrackCustomEventIsInvokedThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new CustomInstrumentation();
            const IActivity emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackCustomEvent(emptyActivity, this.telemetryClient, settings));
        }

        //[Theory(DisplayName =
        //    "GIVEN empty telemetry client WHEN CustomInstrumentation is constructed THEN exception is being thrown")]
        //[AutoData]
        //public void GivenEmptyTelemetryClientWhenCustomInstrumentationIsConstructedThenExceptionIsBeingThrown(
        //    IActivity activity,
        //    InstrumentationSettings settings)
        //{
        //    // Arrange
        //    const TelemetryClient emptyTelemetryClient = null;

        //    // Act
        //    // Assert
        //    Assert.Throws<ArgumentNullException>(() => new CustomInstrumentation(emptyTelemetryClient, settings));
        //}

        //[Fact(DisplayName =
        //    "GIVEN empty settings WHEN CustomInstrumentation is constructed THEN exception is being thrown")]
        //public void GivenEmptySettingsWhenCustomInstrumentationIsConstructedThenExceptionIsBeingThrown()
        //{
        //    // Arrange
        //    const InstrumentationSettings emptySettings = null;

        //    // Act
        //    // Assert
        //    Assert.Throws<ArgumentNullException>(() => new CustomInstrumentation(this.telemetryClient, emptySettings));
        //}
    }
}