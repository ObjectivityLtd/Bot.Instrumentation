namespace Bot.Ibex.Instrumentation.Common.Tests.Instrumentations
{
    using System.Globalization;
    using Bot.Ibex.Instrumentation.Common.Instrumentations;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Bot.Ibex.Instrumentation.Common.Settings;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("QnAInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class QnAInstrumentationTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public QnAInstrumentationTests()
        {
            var telemetryConfiguration =
                new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GIVENAnyActivity_WHENTrackEventIsInvoked_THENEventTelemetryIsBeingSent(
            IActivityAdapter activity,
            QueryResult queryResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation();

            // Act
            instrumentation.TrackEvent(activity, queryResult, settings, this.telemetryClient);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.QnaEvent &&
                    t.Properties[QnAConstants.UserQuery] == activity.MessageActivity.Text &&
                    t.Properties[QnAConstants.KnowledgeBaseQuestion] == queryResult.KnowledgeBaseQuestion &&
                    t.Properties[QnAConstants.KnowledgeBaseAnswer] == queryResult.KnowledgeBaseAnswer &&
                    t.Properties[QnAConstants.Score] == queryResult.Score.ToString(CultureInfo.InvariantCulture))),
                Times.Once);
        }
    }
}