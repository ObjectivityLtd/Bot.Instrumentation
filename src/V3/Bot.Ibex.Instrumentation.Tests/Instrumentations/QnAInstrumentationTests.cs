namespace Bot.Ibex.Instrumentation.V3.Tests.Instrumentations
{
    using System;
    using System.Globalization;
    using AutoFixture.Xunit2;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Connector;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Settings;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using V3.Instrumentations;
    using Xunit;

    [Collection("QnAInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class QnAInstrumentationTests
    {
        public QnAInstrumentationTests()
        {
            var telemetryConfiguration =
                new TelemetryConfiguration(FakeInstrumentationKey, mockTelemetryChannel.Object);
            telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        [Theory(DisplayName = "GIVEN any activity WHEN TrackEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GivenAnyActivity_WhenTrackEventIsInvoked_ThenEventTelemetryIsBeingSent(
            IMessageActivity activity,
            string userQuery,
            string kbQuestion,
            string kbAnswer,
            double score,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation(telemetryClient, settings);

            // Act
            instrumentation.TrackEvent(activity, userQuery, kbQuestion, kbAnswer, score);

            // Assert
            mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.QnaEvent &&
                    t.Properties[QnAConstants.UserQuery] == userQuery &&
                    t.Properties[QnAConstants.KnowledgeBaseQuestion] == kbQuestion &&
                    t.Properties[QnAConstants.KnowledgeBaseAnswer] == kbAnswer &&
                    t.Properties[QnAConstants.Score] == score.ToString(CultureInfo.InvariantCulture))),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackEvent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyActivity_WhenTrackEventIsInvoked_ThenExceptionIsBeingThrown(
            string userQuery,
            string kbQuestion,
            string kbAnswer,
            double score,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation(telemetryClient, settings);
            const IMessageActivity emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
                instrumentation.TrackEvent(emptyActivity, userQuery, kbQuestion, kbAnswer, score));
        }

        [Theory(DisplayName = "GIVEN empty query result WHEN TrackEvent is invoked THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptyQueryResult_WhenTrackEventIsInvoked_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation(telemetryClient, settings);
            IMessageActivity activity = null;
            const string userQuery = null;
            const string kbQuestion = null;
            const string kbAnswer = null;
            const double score = 0;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
                instrumentation.TrackEvent(activity, userQuery, kbQuestion, kbAnswer, score));
        }

        [Theory(DisplayName =
            "GIVEN empty telemetry client WHEN QnAInstrumentation is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyTelemetryClient_WhenQnAInstrumentationIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QnAInstrumentation(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName =
            "GIVEN empty settings WHEN QnAInstrumentation is constructed THEN exception is being thrown")]
        public void GivenEmptySettings_WhenQnAInstrumentationIsConstructed_ThenExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QnAInstrumentation(telemetryClient, emptySettings));
        }
    }
}