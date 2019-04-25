namespace Bot.Ibex.Instrumentation.V4.Tests.Instrumentations
{
    using System;
    using System.Globalization;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Common.Models;
    using Common.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Schema;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using V4.Instrumentations;
    using Xunit;
    using QueryResult = Microsoft.Bot.Builder.AI.QnA.QueryResult;

    [Collection("QnAInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class QnAInstrumentationTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public QnAInstrumentationTests()
        {
            var telemetryConfiguration = new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GIVENAnyActivity_WHENTrackEventIsInvoked_THENEventTelemetryIsBeingSent(
                   IMessageActivity activity,
                   QueryResult queryResult,
                   InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation(this.telemetryClient, settings);

            // Act
            instrumentation.TrackEvent(activity, queryResult);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.QnaEvent &&
                    t.Properties[QnAConstants.UserQuery] == activity.AsMessageActivity().Text &&
                    t.Properties[QnAConstants.KnowledgeBaseQuestion] == string.Join(QuestionsSeparator.Separator, queryResult.Questions) &&
                    t.Properties[QnAConstants.KnowledgeBaseAnswer] == queryResult.Answer &&
                    t.Properties[QnAConstants.Score] == queryResult.Score.ToString(CultureInfo.InvariantCulture))),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackEvent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GIVENEmptyActivity_WHENTrackEventIsInvoked_THENExceptionIsBeingThrown(
            QueryResult queryResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation(this.telemetryClient, settings);
            const IMessageActivity emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackEvent(emptyActivity, queryResult));
        }

        [Theory(DisplayName = "GIVEN empty query result WHEN TrackEvent is invoked THEN exception is being thrown")]
        [AutoMockData]
        public void GIVENEmptyQueryResult_WHENTrackEventIsInvoked_THENExceptionIsBeingThrown(
            IMessageActivity activity,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation(this.telemetryClient, settings);
            const QueryResult emptyQueryResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackEvent(activity, emptyQueryResult));
        }

        [Theory(DisplayName = "GIVEN empty telemetry client WHEN QnAInstrumentation is constructed THEN exception is being thrown")]
        [AutoData]
        public void GIVENEmptyTelemetryClient_WHENQnAInstrumentationIsConstructed_THENExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QnAInstrumentation(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName = "GIVEN empty settings WHEN QnAInstrumentation is constructed THEN exception is being thrown")]
        public void GIVENEmptySettings_WHENQnAInstrumentationIsConstructed_THENExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QnAInstrumentation(this.telemetryClient, emptySettings));
        }
    }
}