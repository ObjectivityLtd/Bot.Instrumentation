﻿namespace Bot.Ibex.Instrumentation.V3.Tests.Instrumentations
{
    using System;
    using AutoFixture.Xunit2;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Connector;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using V3.Instrumentations;
    using Xunit;

    [Collection("SentimentInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class ActivityInstrumentationTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public ActivityInstrumentationTests()
        {
            var telemetryConfiguration =
                new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackActivity is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public async void GivenAnyActivityWhenTrackActivityIsInvokedThenEventTelemetryIsBeingSent(
            IMessageActivity activity,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new ActivityInstrumentation(this.telemetryClient, settings);

            // Act
            await instrumentation.TrackActivity(activity).ConfigureAwait(false);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(x => x.Name.Length == 0)),
                Times.Once);
        }

        [Theory(DisplayName =
            "GIVEN empty activity result WHEN TrackActivity is invoked THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyActivityWhenTrackActivityIsInvokedThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new ActivityInstrumentation(this.telemetryClient, settings);
            const IActivity emptyActivity = null;

            // Act
            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => instrumentation.TrackActivity(emptyActivity))
                .ConfigureAwait(false);
        }

        [Theory(DisplayName =
            "GIVEN empty telemetry client WHEN ActivityInstrumentation is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptyTelemetryClientWhenActivityInstrumentationIsConstructedThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new ActivityInstrumentation(emptyTelemetryClient, settings));
        }

        [Theory(DisplayName =
            "GIVEN empty settings WHEN ActivityInstrumentation is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptySettingsWhenActivityInstrumentationIsConstructedThenExceptionIsBeingThrown(
            TelemetryClient emptyTelemetryClient)
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
                new ActivityInstrumentation(emptyTelemetryClient, emptySettings));
        }
    }
}