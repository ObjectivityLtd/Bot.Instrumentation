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

        [Theory(DisplayName = "GIVEN empty query result WHEN TrackIntent is invoked THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptyQueryResult_WhenTrackIntentIsInvoked_ThenExceptionIsBeingThrown(
            IActivity activity,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new IntentInstrumentation();
            const RecognizedIntentResult emptyLuisResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackIntent(activity, emptyLuisResult, this.telemetryClient, settings));
        }

        //[Theory(DisplayName = "GIVEN empty telemetry client WHEN IntentInstrumentation is constructed THEN exception is being thrown")]
        //[AutoData]
        //public void GivenEmptyTelemetryClient_WhenIntentInstrumentationIsConstructed_ThenExceptionIsBeingThrown(
        //    InstrumentationSettings settings)
        //{
        //    // Arrange
        //    const TelemetryClient emptyTelemetryClient = null;

        //    // Act
        //    // Assert
        //    Assert.Throws<ArgumentNullException>(() => new IntentInstrumentation(emptyTelemetryClient, settings));
        //}

        //[Fact(DisplayName = "GIVEN empty settings WHEN IntentInstrumentation is constructed THEN exception is being thrown")]
        //public void GivenEmptySettings_WhenIntentInstrumentationIsConstructed_ThenExceptionIsBeingThrown()
        //{
        //    // Arrange
        //    const InstrumentationSettings emptySettings = null;

        //    // Act
        //    // Assert
        //    Assert.Throws<ArgumentNullException>(() => new IntentInstrumentation(this.telemetryClient, emptySettings));
        //}
    }
}
