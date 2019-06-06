namespace Bot.Instrumentation.Common.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using AutoFixture.Xunit2;
    using Bot.Instrumentation.Common.Adapters;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Models;
    using Bot.Instrumentation.Common.Sentiments;
    using Bot.Instrumentation.Common.Settings;
    using Bot.Instrumentation.Common.Telemetry;
    using Bot.Instrumentation.Common.Tests.Telemetry;
    using FluentAssertions;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("ActivityExtensions")]
    [Trait("Category", "Extensions")]
    public class ActivityExtensionsTests
    {
        private const string ValidReplyToId = "SOME-REPLY-TO-ID";
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public ActivityExtensionsTests()
        {
            var telemetryConfiguration = new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN Activity WHEN IsIncomingMessage is invoked THEN expected result is being returned")]
        [InlineAutoMockData(ActivityTypes.Message, ValidReplyToId, false)]
        [InlineAutoMockData("MESSAGE", ValidReplyToId, false)] // Case insensitive
        [InlineAutoMockData(ActivityTypes.Message, null, true)]
        [InlineAutoMockData(ActivityTypes.Invoke, ValidReplyToId, false)]
        public void GIVENActivity_WHENIsIncomingMessageIsInvoked_THENExpectedResultIsBeingReturned(
            string type,
            string replyToId,
            bool expectedResult,
            Activity activity)
        {
            // Arrange
            activity.ReplyToId = replyToId;
            activity.Type = type;

            // Act
            var actualResult = activity.IsIncomingMessage();

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "GIVEN Activity WHEN ToSentimentInput is invoked THEN SentimentInput is being returned")]
        [AutoMockData]
        public void GIVENActivity_WHENToSentimentInputIsInvoked_THENSentimentInputIsBeingReturned(
            IActivityAdapter activity)
        {
            // Arrange
            const int expectedNumberOfDocuments = 1;
            const string expectedSentimentInputId = "1";

            // Act
            var actualResult = activity.ToSentimentInput();

            // Assert
            actualResult.Documents.Should().HaveCount(expectedNumberOfDocuments);
            actualResult.Documents.Should().ContainSingle(i =>
                i.Text == activity.MessageActivity.Text &&
                i.Id == expectedSentimentInputId);
        }

        [Fact(DisplayName = "GIVEN empty Activity WHEN IsIncomingMessage is invoked THEN False is being returned")]
        public void GIVENEmptyActivity_WHENIsIncomingMessageIsInvoked_THENFalseIsBeingReturned()
        {
            // Arrange
            const IActivityAdapter activity = null;
            const bool expectedResult = false;

            // Act
            var actualResult = activity.IsIncomingMessage();

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        [Fact(DisplayName = "GIVEN empty Activity WHEN ToSentimentInput is invoked THEN empty SentimentInput is being returned")]
        public void GIVENEmptyActivity_WHENToSentimentInputIsInvoked_THENEmptySentimentInputIsBeingReturned()
        {
            // Arrange
            const IActivityAdapter activity = null;

            // Act
            var actualResult = activity.ToSentimentInput();

            // Assert
            actualResult.Should().BeNull();
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackCustomEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GIVENAnyActivity_WHENTrackCustomEventIsInvoked_THENEventTelemetryIsBeingSent(
            IActivityAdapter activity,
            InstrumentationSettings settings)
        {
            // Arrange
            // Act
            activity.TrackCustomEvent(this.telemetryClient, settings);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.CustomEvent)),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN any activity, any event name and any property WHEN TrackCustomEvent is invoked THEN event telemetry is being sent")]
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
            var properties = new Dictionary<string, string> { { propertyKey, propertyValue } };

            // Act
            activity.TrackCustomEvent(this.telemetryClient, settings, eventName, properties);

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
            const IActivityAdapter emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => emptyActivity.TrackCustomEvent(this.telemetryClient, settings));
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackIntent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GIVENAnyActivity_WHENTrackIntentIsInvoked_THENEventTelemetryIsBeingSent(
            IActivityAdapter activity,
            IntentResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            // Act
            activity.TrackIntent(luisResult, this.telemetryClient, settings);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.Intent &&
                    t.Properties[IntentConstants.Intent] == luisResult.Intent &&
                    t.Properties[IntentConstants.Score] == luisResult.Score &&
                    t.Properties[IntentConstants.Entities] == luisResult.Entities)),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN any activity, any result and any property WHEN TrackIntent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void
            GIVENAnyActivityAnyResultAnAnyProperty_WHENTrackIntentIsInvoked_THENEventTelemetryIsBeingSent(
                IActivityAdapter activity,
                IntentResult result,
                InstrumentationSettings settings)
        {
            // Arrange
            // Act
            activity.TrackIntent(result, this.telemetryClient, settings);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.Intent &&
                    t.Properties[IntentConstants.Intent] == result.Intent &&
                    t.Properties[IntentConstants.Score] == result.Score &&
                    t.Properties[IntentConstants.Entities] == result.Entities)),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity result WHEN TrackIntent is invoked THEN exception is being thrown")]
        [AutoData]
        public void GIVENEmptyActivity_WHENTrackIntentIsInvoked_THENExceptionIsBeingThrown(
            IntentResult luisResult,
            InstrumentationSettings settings)
        {
            // Arrange
            const IActivityAdapter emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => emptyActivity.TrackIntent(luisResult, this.telemetryClient, settings));
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GIVENAnyActivity_WHENTrackEventIsInvoked_THENEventTelemetryIsBeingSent(
            IActivityAdapter activity,
            QueryResult queryResult,
            InstrumentationSettings settings)
        {
            // Arrange
            // Act
            activity.TrackEvent(queryResult, settings, this.telemetryClient);

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

        [Theory(DisplayName = "GIVEN any activity WHEN TrackMessageSentiment is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public async void GIVENAnyActivity_WHENTrackMessageSentimentIsInvoked_THENEventTelemetryIsBeingSent(
            double sentimentScore,
            IActivityAdapter activity,
            ISentimentClient sentimentClient,
            InstrumentationSettings settings)
        {
            // Arrange
            Mock.Get(sentimentClient)
               .Setup(s => s.GetSentiment(activity))
               .Returns(Task.FromResult<double?>(sentimentScore));

            // Act
            await activity.TrackMessageSentiment(this.telemetryClient, settings, sentimentClient)
                .ConfigureAwait(false);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.MessageSentiment &&
                    t.Properties[SentimentConstants.Score] == sentimentScore.ToString(CultureInfo.InvariantCulture))),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN empty activity WHEN TrackMessageSentiment is invoked THEN exception is being thrown")]
        [AutoMockData]
        public async void GIVENEmptyActivity_WHENTrackMessageSentimentIsInvoked_THENExceptionIsBeingThrown(
            ISentimentClient sentimentClient,
            InstrumentationSettings settings)
        {
            // Arrange
            const IActivityAdapter emptyActivity = null;

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => emptyActivity.TrackMessageSentiment(this.telemetryClient, settings, sentimentClient))
                .ConfigureAwait(false);
        }
    }
}
