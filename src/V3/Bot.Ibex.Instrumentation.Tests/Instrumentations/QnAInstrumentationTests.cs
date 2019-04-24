namespace Bot.Ibex.Instrumentation.V3.Tests.Instrumentations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Common.Constants;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Microsoft.Bot.Connector;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;
    using QnAInstrumentation = V3.Instrumentations.QnAInstrumentation;

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
        public void GivenAnyActivityWhenTrackEventIsInvokedThenEventTelemetryIsBeingSent(
                   IMessageActivity activity,
                   InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation(this.telemetryClient, settings);
            QnAMakerResults queryResult = new QnAMakerResults
            {
                Answers = new List<QnAMakerResult>
                {
                    new QnAMakerResult
                    {
                        Score = .5,
                        Questions = new List<string>
                        {
                            "good",
                            "bad"
                        },
                        Answer = "good"
                    }
                }
            };

            // Act
            instrumentation.TrackEvent(activity, queryResult);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.QnaEvent &&
                    t.Properties[QnAConstants.UserQuery] == activity.AsMessageActivity().Text &&
                    t.Properties[QnAConstants.KnowledgeBaseQuestion] == string.Join(QnAInstrumentation.QuestionsSeparator, queryResult.Answers.First().Questions) &&
                    t.Properties[QnAConstants.KnowledgeBaseAnswer] == queryResult.Answers.First().Answer &&
                    t.Properties[QnAConstants.Score] == queryResult.Answers.First().Score.ToString(CultureInfo.InvariantCulture))),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackEvent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyActivityWhenTrackEventIsInvokedThenExceptionIsBeingThrown(
            QnAMakerResults queryResult,
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
        public void GivenEmptyQueryResultWhenTrackEventIsInvokedThenExceptionIsBeingThrown(
            IMessageActivity activity,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation(this.telemetryClient, settings);
            const QnAMakerResults emptyQueryResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => instrumentation.TrackEvent(activity, emptyQueryResult));
        }

        [Theory(DisplayName = "GIVEN empty telemetry client WHEN QnAInstrumentation is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyTelemetryClientWhenQnAInstrumentationIsConstructedThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QnAInstrumentation(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName = "GIVEN empty settings WHEN QnAInstrumentation is constructed THEN exception is being thrown")]
        public void GivenEmptySettingsWhenQnAInstrumentationIsConstructedThenExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QnAInstrumentation(this.telemetryClient, emptySettings));
        }
    }
}
